using System.Diagnostics.CodeAnalysis;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.TvShows;

namespace Proxarr.Api.Core
{
    public interface ITmdbProxy
    {
        Task<Movie> GetMovieAsync(int movieId, MovieMethods extraMethods = MovieMethods.Undefined, CancellationToken cancellationToken = default);
        Task<TvShow> GetTvShowAsync(int id, TvShowMethods extraMethods = TvShowMethods.Undefined, string language = null, string includeImageLanguage = null, CancellationToken cancellationToken = default);
    }

    [ExcludeFromCodeCoverage]
    public class TmdbProxy : ITmdbProxy
    {
        private readonly TMDbClient _tMDbClient;

        public TmdbProxy(TMDbClient tMDbClient)
        {
            ArgumentNullException.ThrowIfNull(tMDbClient);
            _tMDbClient = tMDbClient;
        }

        public Task<TvShow> GetTvShowAsync(int id, TvShowMethods extraMethods = TvShowMethods.Undefined, string language = null, string includeImageLanguage = null, CancellationToken cancellationToken = default)
        {
            return _tMDbClient.GetTvShowAsync(id, extraMethods, language, includeImageLanguage, cancellationToken);
        }

        public Task<Movie> GetMovieAsync(int movieId, MovieMethods extraMethods = MovieMethods.Undefined, CancellationToken cancellationToken = default)
        {
            return _tMDbClient.GetMovieAsync(movieId, extraMethods, cancellationToken);
        }
    }
}
