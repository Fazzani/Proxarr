using Proxarr.Api.Models;
using System.Diagnostics.CodeAnalysis;

namespace Proxarr.Api.Services
{
    public interface ISonarrService
    {
        /// <summary>
        /// Full scan of Sonarr library for qualifying tv shows
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        Task FullScan(CancellationToken cancellationToken);

        /// <summary>
        /// Qualify tv show
        /// </summary>
        /// <param name="tvAdded"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> Qualify(TvAdded tvAdded, CancellationToken cancellationToken);
    }
}