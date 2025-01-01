using Microsoft.Extensions.Options;
using Proxarr.Api.Configuration;
using Proxarr.Api.Core;
using Proxarr.Api.Core.Extensions;
using Proxarr.Api.Models;
using Sonarr.Http.Client;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Proxarr.Api.Services
{
    public class SonarrService : ISonarrService
    {
        private readonly ILogger<SonarrService> _logger;
        private readonly ITmdbProxy _tMDbClient;
        private readonly AppConfiguration _appConfiguration;
        private readonly SonarrClient _sonarrClient;

        public SonarrService(ILogger<SonarrService> logger,
                             ITmdbProxy tMDbClient,
                             IOptions<AppConfiguration> appConfiguration,
                             SonarrClient sonarrClient)
        {
            _logger = logger;
            _tMDbClient = tMDbClient;
            _appConfiguration = appConfiguration?.Value ?? throw new ArgumentNullException(nameof(appConfiguration));
            _sonarrClient = sonarrClient;
        }

        //<inheritdoc/>
        [ExcludeFromCodeCoverage(Justification ="Is tested with Qualify function")]
        public async Task FullScan(CancellationToken cancellationToken)
        {
            foreach (var client in _appConfiguration.Clients.Where(x => x.IsSonarr))
            {
                _sonarrClient.BaseUrl = client.BaseUrl;

                var series = await _sonarrClient
                    .SeriesAll2Async(null, null, cancellationToken)
                    .ConfigureAwait(false);

                foreach (var tv in series)
                {
                    _logger.LogInformation("Rescanning TV {Title}", tv.Title);

                    await Qualify(new TvAdded
                    {
                        ApplicationUrl = _sonarrClient.BaseUrl,
                        EventType = "FULL_SCAN",
                        Series = new Series { Id = tv.Id, TmdbId = tv.TmdbId, Title = tv.Title }
                    }, cancellationToken);
                }
            }
        }

        //<inheritdoc/>
        public async Task<string> Qualify(TvAdded tvAdded, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(tvAdded);

            _logger.LogInformation("Qualifying tv {Title}", tvAdded.Series.Title);

            var tmdbItem = await _tMDbClient
                .GetTvShowAsync(tvAdded.Series.TmdbId, cancellationToken, TMDbLib.Objects.TvShows.TvShowMethods.WatchProviders)
                .ConfigureAwait(false);

            if (tmdbItem != null)
            {
                _sonarrClient.BaseUrl = tvAdded.ApplicationUrl;
                var seriesSonarr = await _sonarrClient.SeriesGETAsync(tvAdded.Series.Id, false, cancellationToken).ConfigureAwait(false);

                if (seriesSonarr == null)
                {
                    _logger.LogError("Tv not found in Sonarr {Title}", tvAdded.Series.Title);
                    return "NotFound";
                }

                await UpdateTags(tmdbItem, seriesSonarr, cancellationToken).ConfigureAwait(false);
                await _sonarrClient
                .SeriesPUTAsync(false, seriesSonarr.Id.ToString(CultureInfo.InvariantCulture), seriesSonarr, cancellationToken)
                .ConfigureAwait(false);
            }
            else
            {
                return "NotFound";
            }

            return string.Empty;
        }

        private async Task UpdateTags(TMDbLib.Objects.TvShows.TvShow tmdbItem,
                                      SeriesResource seriesSonarr,
                                      CancellationToken cancellationToken)
        {
            var matched = false;
            var existedTags = await _sonarrClient.TagAllAsync(cancellationToken).ConfigureAwait(false);

            foreach (var provider in _appConfiguration.WatchProvidersDict)
            {
                if (tmdbItem.WatchProviders?.Results.TryGetValue(provider.Key, out var matchedProvider) == true)
                {
                    foreach (var pr in provider.Value)
                    {
                        if (matchedProvider.Free?.Any(x => x.ProviderName.Equals(pr, StringComparison.OrdinalIgnoreCase)) == true ||
                            matchedProvider.FlatRate?.Any(x => x.ProviderName.Equals(pr, StringComparison.OrdinalIgnoreCase)) == true)
                        {
                            _logger.LogInformation("Matched Free/FlatRate provider {WatchProvider} for {Title}", pr, seriesSonarr.Title);
                            matched |= await AddTag(seriesSonarr, matched, existedTags, pr, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
            }

            if (!matched)
            {
                await AddTag(seriesSonarr, matched, existedTags, _appConfiguration.TagName!, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<bool> AddTag(SeriesResource seriesSonarr,
                                        bool updated,
                                        ICollection<TagResource> existingTags,
                                        string providerTag,
                                        CancellationToken cancellationToken)
        {
            var tag = existingTags.FirstOrDefault(x => x.Label.Equals(providerTag, StringComparison.OrdinalIgnoreCase));

            if (tag is null)
            {
                tag = new TagResource { Label = providerTag };
                tag = await _sonarrClient.TagPOSTAsync(tag.Slugify(), cancellationToken).ConfigureAwait(false);
            }

            if (!seriesSonarr.Tags.Contains(tag.Id))
            {
                _logger.LogInformation("Adding tag {Tag} for {Title}", tag.Label, seriesSonarr.Title);
                seriesSonarr.Tags.Add(tag.Id);
                return true;
            }

            return updated;
        }
    }
}
