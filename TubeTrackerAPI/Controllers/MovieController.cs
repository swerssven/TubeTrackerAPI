using Microsoft.AspNetCore.Mvc;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.Middleware;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.Repositories;

namespace TubeTrackerAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TubeTrackerDbContext _dbContext;

        public MovieController(IConfiguration configuration, TubeTrackerDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        // GET api/<UserController>
        [Route("getMovieList")]
        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery]string filter, [FromQuery]int page, [FromQuery]string language)
        {
            return Ok(await new MovieService(_dbContext).GetMovieSearchList(filter, page, language));
        }


        // GET api/<UserController>/5
        /*[HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            MovieService movieService = new MovieService(_dbContext);
            return Ok(await movieService.GetMovie(id));
        }*/

        // GET api/<UserController>?id=5&language=es-ES
        [Route("getMovie")]
        [HttpGet]
        public async Task<IActionResult> GetMovieAsync([FromQuery] int id, [FromQuery] string language)
        {
            MovieService movieService = new MovieService(this._dbContext);
            
            return Ok(await movieService.CreateMovie(id, language));
        }

    }
}
