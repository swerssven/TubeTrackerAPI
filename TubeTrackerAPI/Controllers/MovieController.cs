using Microsoft.AspNetCore.Mvc;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.Middleware;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.Models.Request;

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

        // GET api/<MovieController>
        [Route("getMovieList")]
        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery]string filter, [FromQuery]int page, [FromQuery]string language)
        {
            return Ok(await new MovieService(_dbContext).GetMovieSearchList(filter, page, language));
        }

        // GET api/<MovieController>?id=5&language=es-ES
        [Route("getMovie")]
        [HttpGet]
        public async Task<IActionResult> GetMovieAsync([FromQuery] int id, [FromQuery] string language)
        {
            MovieService movieService = new MovieService(this._dbContext);
            
            return Ok(await movieService.CreateMovie(id, language));
        }

        // GET api/<MovieController>/getReviews?movieApiId=5
        [Route("getReviews")]
        [HttpGet]
        public async Task<IActionResult> GetReviewsAsync([FromQuery] int movieApiId)
        {
            MovieService movieService = new MovieService(this._dbContext);

            return Ok(await movieService.GetMovieReviews(movieApiId));
        }

        // POST api/<MovieController>/createReview
        [Route("createReview")]
        [HttpPost]
        public async Task<IActionResult> CreateMovieReviewListAsync([FromBody] CreateMovieReviewListRequest request)
        {
            MovieService movieService = new MovieService(this._dbContext);

            return Ok(await movieService.CreateMovieReviewList(request));
        }

    }
}
