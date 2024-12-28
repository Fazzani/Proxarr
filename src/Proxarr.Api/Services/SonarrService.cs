using Microsoft.Extensions.Options;
using Proxarr.Api.Models;
using Sonarr.Http.Client;
using System.Globalization;
using TMDbLib.Client;

namespace Proxarr.Api.Services
{
    public class SonarrService : ISonarrService
    {
        private readonly ILogger<SonarrService> _logger;
        private readonly TMDbClient _tMDbClient;
        private readonly AppConfiguration _appConfiguration;
        private readonly SonarrClient _sonarrClient;

        public SonarrService(ILogger<SonarrService> logger,
                             TMDbClient tMDbClient,
                             IOptions<AppConfiguration> appConfiguration,
                             SonarrClient sonarrClient)
        {
            _logger = logger;
            _tMDbClient = tMDbClient;
            _appConfiguration = appConfiguration?.Value ?? throw new ArgumentNullException(nameof(appConfiguration));
            _sonarrClient = sonarrClient;
        }

        //<inheritdoc/>
        public async Task<string> Qualify(TvAdded tvAdded, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(tvAdded);

            _logger.LogInformation("Qualifying tv {Title}", tvAdded.Series.Title);
            // TODO: validation

            var tmdbItem = await _tMDbClient
                .GetTvShowAsync(tvAdded.Series.TmdbId, TMDbLib.Objects.TvShows.TvShowMethods.WatchProviders, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (tmdbItem != null && tmdbItem.WatchProviders.Results?.Count > 0)
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

            return string.Empty;
        }

        private async Task UpdateTags(TMDbLib.Objects.TvShows.TvShow tmdbItem,
                                            SeriesResource seriesSonarr,
                                            CancellationToken cancellationToken)
        {
            var matched = false;
            var existingTags = await _sonarrClient.TagAllAsync(cancellationToken).ConfigureAwait(false);

            foreach (var provider in _appConfiguration.WatchProviders)
            {
                if (tmdbItem.WatchProviders.Results.TryGetValue(provider.Key, out var matchedProvider))
                {
                    foreach (var pr in provider.Value)
                    {
                        var providerTag = pr.Replace(" ", "_");

                        if (matchedProvider.Free?.Any(x => x.ProviderName.Equals(pr, StringComparison.OrdinalIgnoreCase)) == true ||
                            matchedProvider.FlatRate?.Any(x => x.ProviderName.Equals(pr, StringComparison.OrdinalIgnoreCase)) == true)
                        {
                            _logger.LogInformation("Matched Free/FlatRate provider {WatchProvider} for {Title}", pr, seriesSonarr.Title);
                            matched |= await AddTag(seriesSonarr, matched, existingTags, providerTag, cancellationToken).ConfigureAwait(false);
                        }
                        if (matchedProvider.Buy?.Any(x => x.ProviderName.Equals(pr, StringComparison.OrdinalIgnoreCase)) == true)
                        {
                            _logger.LogInformation("Found buy provider {WatchProvider} for {Title}", pr, seriesSonarr.Title);
                        }
                        if (matchedProvider.Rent?.Any(x => x.ProviderName.Equals(pr, StringComparison.OrdinalIgnoreCase)) == true)
                        {
                            _logger.LogInformation("Found rent provider {WatchProvider} for {Title}", pr, seriesSonarr.Title);
                        }
                    }
                }
            }

            if (!matched)
            {
                await AddTag(seriesSonarr, matched, existingTags, _appConfiguration.TAG_NAME, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<bool> AddTag(SeriesResource seriesSonarr,
                                        bool updated,
                                        ICollection<TagResource> existingTags,
                                        string providerTag,
                                        CancellationToken cancellationToken)
        {
            providerTag = providerTag.Replace(" ", "_");

            var tag = existingTags.FirstOrDefault(x => x.Label.Equals(providerTag, StringComparison.OrdinalIgnoreCase));

            if (tag is null)
            {
                tag = new TagResource { Label = providerTag };
                tag = await _sonarrClient.TagPOSTAsync(tag, cancellationToken).ConfigureAwait(false);
            }

            if (!seriesSonarr.Tags.Contains(tag.Id))
            {
                _logger.LogInformation("Adding tag {Tag} for {Title}", tag.Label, seriesSonarr.Title);
                seriesSonarr.Tags.Add(tag.Id);
                updated = true;
            }

            return updated;
        }
    }
}
