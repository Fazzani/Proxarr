using Microsoft.AspNetCore.Mvc;
using Proxarr.Api.Models;
using Proxarr.Api.Services;

namespace Proxarr.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Qualified(MediaAdded media, CancellationToken cancellationToken)
        {
            if (media.EventType.Equals("test", StringComparison.OrdinalIgnoreCase))
            {
                return Ok();
            }

            if(media is MovieAdded movie)
            {
                var result = await _radarrService.Qualify(movie, cancellationToken).ConfigureAwait(false);

                if (result == nameof(NotFound))
                {
                    _logger.LogError("Movie not found in Radarr {Title}", movie.Movie.Title);
                    return NotFound();
                }
            }
            else if (media is TvAdded tvAdded)
            {
                var result = await _sonarrService.Qualify(tvAdded, cancellationToken).ConfigureAwait(false);
                if (result == nameof(NotFound))
                {
                    _logger.LogError("Tv not found in Sonarr {Title}", tvAdded.Series.Title);
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
