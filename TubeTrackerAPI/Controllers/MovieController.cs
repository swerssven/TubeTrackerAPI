using Microsoft.AspNetCore.Mvc;
using TubeTrackAPI.Models;
using TubeTrackAPI.Repositories;
using TubeTrackAPI.Services;

namespace TubeTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery]string filter, [FromQuery]int page)
        {
            return Ok(await new MovieService().GetMovieSearchList(filter, page));
        }

    }
}
