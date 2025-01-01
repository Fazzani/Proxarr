using System.Diagnostics.CodeAnalysis;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.TvShows;

namespace Proxarr.Api.Core
{
    public interface ITmdbProxy
    {
        Task<Movie> GetMovieAsync(int movieId, MovieMethods extraMethods = MovieMethods.Undefined);
        Task<Movie> GetMovieAsync(int movieId, CancellationToken cancellationToken, MovieMethods extraMethods = MovieMethods.Undefined);
        Task<TvShow> GetTvShowAsync(int id, TvShowMethods extraMethods = TvShowMethods.Undefined);
        Task<TvShow> GetTvShowAsync(int id, CancellationToken cancellationToken, TvShowMethods extraMethods = TvShowMethods.Undefined);
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

        public Task<TvShow> GetTvShowAsync(int id, TvShowMethods extraMethods = TvShowMethods.Undefined)
        {
            return _tMDbClient.GetTvShowAsync(id, extraMethods, null, null, CancellationToken.None);
        }

        public Task<TvShow> GetTvShowAsync(int id,  CancellationToken cancellationToken, TvShowMethods extraMethods = TvShowMethods.Undefined)
        {
            return _tMDbClient.GetTvShowAsync(id, extraMethods, null, null, cancellationToken);
        }

        public Task<Movie> GetMovieAsync(int movieId, MovieMethods extraMethods = MovieMethods.Undefined)
        {
            return _tMDbClient.GetMovieAsync(movieId, extraMethods, CancellationToken.None);
        }

        public Task<Movie> GetMovieAsync(int movieId, CancellationToken cancellationToken, MovieMethods extraMethods = MovieMethods.Undefined)
        {
            return _tMDbClient.GetMovieAsync(movieId, extraMethods, cancellationToken);
        }
    }
}
