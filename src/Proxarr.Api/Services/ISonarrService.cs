using Proxarr.Api.Models;

namespace Proxarr.Api.Services
{
    public interface ISonarrService
    {
        Task<string> Qualify(TvAdded tvAdded, CancellationToken cancellationToken);
    }
}