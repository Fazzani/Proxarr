using Proxarr.Api.Models;

namespace Proxarr.Api.Services
{
    public interface IRadarrService
    {
        /// <summary>
        /// Full scan of Radarr library for qualifying movies
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task FullScan(CancellationToken cancellationToken);

        /// <summary>
        /// Qualify movie
        /// </summary>
        /// <param name="movieAdded"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> Qualify(MovieAdded movieAdded, CancellationToken cancellationToken);
    }
}