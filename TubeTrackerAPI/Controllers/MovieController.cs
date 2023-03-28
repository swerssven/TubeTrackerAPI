using Microsoft.AspNetCore.Mvc;
using TubeTrackAPI.Services;
using TubeTrackerAPI.Middleware;

namespace TubeTrackAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MovieController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery]string filter, [FromQuery]int page)
        {
            return Ok(await new MovieService().GetMovieSearchList(filter, page));
        }

    }
}
