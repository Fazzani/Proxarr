using Microsoft.AspNetCore.Mvc;
using Proxarr.Api.Models;
using Proxarr.Api.Services;

namespace Proxarr.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QualifierController : ControllerBase
    {
        private readonly ILogger<QualifierController> _logger;
        private readonly IRadarrService _radarrService;
        private readonly ISonarrService _sonarrService;

        public QualifierController(ILogger<QualifierController> logger,
                                   IRadarrService radarrService,
                                   ISonarrService sonarrService)
        {
            _logger = logger;
            _radarrService = radarrService;
            _sonarrService = sonarrService;
        }

        [HttpPost("movie")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> QualifiedMovie(MovieAdded movie, CancellationToken cancellationToken)
        {
            var result = await _radarrService.Qualify(movie, cancellationToken).ConfigureAwait(false);
            if (result == nameof(NotFound))
            {
                _logger.LogError("Movie not found in Radarr {Title}", movie.Movie.Title);
                return NotFound();
            }
            return Ok();
        }

        [HttpPost("tv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> QualifiedTv(TvAdded tvAdded, CancellationToken cancellationToken)
        {
            var result = await _sonarrService.Qualify(tvAdded, cancellationToken).ConfigureAwait(false);
            if (result == nameof(NotFound))
            {
                _logger.LogError("Tv not found in Sonarr {Title}", tvAdded.Series.Title);
                return NotFound();
            }
            return Ok();
        }
    }
}
