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

        // GET api/<MovieController>/getMovieSearchList?filter=Avatar&page=1&language=es-ES
        [Route("getMovieSearchList")]
        [HttpGet]
        public async Task<IActionResult> GetMovieSearchListAsync([FromQuery]string filter, [FromQuery]int page, [FromQuery]string language)
        {
            return Ok(await new MovieService(_dbContext).GetMovieSearchList(filter, page, language));
        }

        // GET api/<MovieController>/getMoviePopularList?page=1&language=es-ES
        [Route("getMoviePopularList")]
        [HttpGet]
        public async Task<IActionResult> GetMoviePopularListAsync([FromQuery] int page, [FromQuery] string language)
        {
            return Ok(await new MovieService(_dbContext).GetMoviePopularList(page, language));
        }

        // GET api/<MovieController>getMovieTopRatedList?page=1&language=es-ES
        [Route("getMovieTopRatedList")]
        [HttpGet]
        public async Task<IActionResult> GetMovieTopRatedListAsync([FromQuery] string language)
        {
            return Ok(await new MovieService(_dbContext).GetMovieTopRatedList(language));
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

        // POST api/<MovieController>/setMovieRating?movieApiId=76600&userId=1&rating=5
        [Route("setMovieRating")]
        [HttpPost]
        public async Task<IActionResult> SetMovieRating([FromQuery] int movieApiId, [FromQuery] int userId, [FromQuery] int rating)
        {
            MovieService movieService = new MovieService(_dbContext);

            return Ok(await movieService.SetMovieRating(movieApiId, userId, rating));
        }

        // GET api/<MovieController>/getMovieRatings?userId=1&movieApiId=5
        [Route("getMovieRatings")]
        [HttpGet]
        public async Task<IActionResult> GetReviewsAsync([FromQuery] int userId, [FromQuery] int movieApiId)
        {
            MovieService movieService = new MovieService(this._dbContext);

            return Ok(await movieService.GetMovieRatings(userId, movieApiId));
        }
    }
}
