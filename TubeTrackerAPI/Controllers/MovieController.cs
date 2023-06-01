using Microsoft.AspNetCore.Mvc;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Middleware;

namespace TubeTrackerAPI.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetMovieSearchListAsync([FromQuery]string filter, [FromQuery]int page, [FromQuery]string language, [FromQuery]int userId)
        {
            return Ok(await new MovieService(_dbContext).GetMovieSearchList(filter, page, language, userId));
        }

        // GET api/<MovieController>/getMoviePopularList?page=1&language=es-ES
        [Route("getMoviePopularList")]
        [HttpGet]
        public async Task<IActionResult> GetMoviePopularListAsync([FromQuery] string language, [FromQuery] int userId)
        {
            return Ok(await new MovieService(_dbContext).GetMoviePopularList(language, userId));
        }

        // GET api/<MovieController>getMovieTopRatedList?page=1&language=es-ES
        [Route("getMovieTopRatedList")]
        [HttpGet]
        public async Task<IActionResult> GetMovieTopRatedListAsync([FromQuery] string language, [FromQuery] int userId)
        {
            return Ok(await new MovieService(_dbContext).GetMovieTopRatedList(language, userId));
        }

        // GET api/<MovieController>?id=5&language=es-ES
        [Route("getMovie")]
        [HttpGet]
        public async Task<IActionResult> GetMovieAsync([FromQuery] int id, [FromQuery] string language, [FromQuery] int userId)
        {
            MovieService movieService = new MovieService(this._dbContext);
            
            return Ok(await movieService.CreateMovie(id, language, userId));
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

        //DELETE api/<MovieController>/DeleteMovieReview?movieReviewId=3
        [Route("DeleteMovieReview")]
        [HttpDelete]
        public async Task<IActionResult> DeleteMovieReview([FromQuery] int movieReviewId)
        {
            MovieService movieService = new MovieService(this._dbContext);

            return Ok(await movieService.DeleteMovieReview(movieReviewId));
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

        // POST api/<MovieController>/setMovieWatched?movieApiId=76600&userId=1&watched=true
        [Route("setMovieWatched")]
        [HttpPost]
        public async Task<IActionResult> setMovieWatched([FromQuery] int movieApiId, [FromQuery] int userId, [FromQuery]string language, [FromQuery] bool watched)
        {
            MovieService movieService = new MovieService(_dbContext);

            return Ok(await movieService.setMovieWatched(movieApiId, userId, language, watched));
        }

        // POST api/<MovieController>/setMovieFavorite?movieApiId=76600&userId=1&favorite=true
        [Route("setMovieFavorite")]
        [HttpPost]
        public async Task<IActionResult> setMovieFavorite([FromQuery] int movieApiId, [FromQuery] int userId, [FromQuery] string language, [FromQuery] bool favorite)
        {
            MovieService movieService = new MovieService(_dbContext);

            return Ok(await movieService.setMovieFavorite(movieApiId, userId, language, favorite));
        }

        // GET api/<MovieController>/getMovieFavoritesList?userId=1&language=es-ES
        [Route("getMovieFavoritesList")]
        [HttpGet]
        public async Task<IActionResult> getMovieFavoritesList([FromQuery] int userId, [FromQuery] string language)
        {
            MovieService movieService = new MovieService(this._dbContext);

            return Ok(await movieService.getMovieFavoritesList(userId, language));
        }

        // GET api/<MovieController>/getLastWatchedMoviesList?userId=1&language=Es-ES
        [Route("getLastWatchedMoviesList")]
        [HttpGet]
        public async Task<IActionResult> getLastWatchedMoviesList([FromQuery] int userId, [FromQuery] string language)
        {
            MovieService movieService = new MovieService(this._dbContext);

            return Ok(await movieService.getLastWatchedMoviesList(userId, language));
        }
    }
}
