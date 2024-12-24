using Proxarr.Api.Models;

namespace Proxarr.Api.Services
{
    public interface IRadarrService
    {
        Task<string> Qualify(MovieAdded movieAdded, CancellationToken cancellationToken);
    }
}