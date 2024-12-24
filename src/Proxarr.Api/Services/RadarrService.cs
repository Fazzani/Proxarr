using Proxarr.Api.Models;
using Radarr.Http.Client;
using TMDbLib.Client;

namespace Proxarr.Api.Services
{
    public class RadarrService : IRadarrService
    {
        private readonly ILogger<RadarrService> _logger;
        private readonly TMDbClient _tMDbClient;
        private readonly AppConfiguration _appConfiguration;
        private readonly RadarrClient _radarrClient;

        public RadarrService(ILogger<RadarrService> logger,
                             TMDbClient tMDbClient,
                             AppConfiguration appConfiguration,
                             RadarrClient radarrClient)
        {
            _logger = logger;
            _tMDbClient = tMDbClient;
            _appConfiguration = appConfiguration;
            _radarrClient = radarrClient;
        }

        //<inheritdoc/>
        public async Task<string> Qualify(MovieAdded movieAdded, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(movieAdded);

            _logger.LogInformation("Qualifying movie {Title}", movieAdded.Movie.Title);

            var tmdbItem = await _tMDbClient
                .GetMovieAsync(movieAdded.Movie.TmdbId, TMDbLib.Objects.Movies.MovieMethods.WatchProviders, cancellationToken)
                .ConfigureAwait(false);

            if (tmdbItem != null && tmdbItem.WatchProviders.Results?.Count > 0)
            {
                _radarrClient.BaseUrl = movieAdded.ApplicationUrl;
                var movieRadarr = await _radarrClient.MovieGET2Async(movieAdded.Movie.Id, cancellationToken).ConfigureAwait(false);

                if (movieRadarr == null)
                {
                    _logger.LogError("Movie not found in Radarr {Title}", movieAdded.Movie.Title);
                    return "NotFound";
                }

                if (await UpdateTags(tmdbItem, movieRadarr, cancellationToken).ConfigureAwait(false))
                {
                    await _radarrClient
                    .MoviePUTAsync(false, movieRadarr.Id.ToString(), movieRadarr, cancellationToken)
                    .ConfigureAwait(false);
                }
            }

            return string.Empty;
        }

        private async Task<bool> UpdateTags(TMDbLib.Objects.Movies.Movie tmdbItem,
                                            MovieResource movieRadarr,
                                            CancellationToken cancellationToken)
        {
            var updated = false;
            var existingTags = await _radarrClient.TagAllAsync(cancellationToken).ConfigureAwait(false);

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
                            _logger.LogInformation("Found Free/FlatRate provider {WatchProvider} for {Title}", pr, movieRadarr.Title);
                            updated = await AddTag(movieRadarr, updated, existingTags, providerTag, cancellationToken).ConfigureAwait(false);
                        }
                        if (matchedProvider.Buy?.Any(x => x.ProviderName.Equals(pr, StringComparison.OrdinalIgnoreCase)) == true)
                        {
                            _logger.LogInformation("Found buy provider {WatchProvider} for {Title}", pr, movieRadarr.Title);
                        }
                        if (matchedProvider.Rent?.Any(x => x.ProviderName.Equals(pr, StringComparison.OrdinalIgnoreCase)) == true)
                        {
                            _logger.LogInformation("Found rent provider {WatchProvider} for {Title}", pr, movieRadarr.Title);
                        }
                    }
                }
            }

            if (updated)
            {
                await AddTag(movieRadarr, updated, existingTags, _appConfiguration.QualifiedTagName, cancellationToken).ConfigureAwait(false);
            }

            return updated;
        }

        private async Task<bool> AddTag(MovieResource movieRadarr,
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
                tag = await _radarrClient.TagPOSTAsync(tag, cancellationToken).ConfigureAwait(false);
            }

            if (!movieRadarr.Tags.Contains(tag.Id))
            {
                _logger.LogInformation("Adding tag {Tag} for {Title}", tag.Label, movieRadarr.Title);
                movieRadarr.Tags.Add(tag.Id);
                updated = true;
            }

            return updated;
        }
    }
}
