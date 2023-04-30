using Microsoft.AspNetCore.Mvc;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerieController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TubeTrackerDbContext _dbContext;

        public SerieController(IConfiguration configuration, TubeTrackerDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        // GET api/<SerieController>/getSerieSearchList?filter=BigBang&page=1&language=es-ES
        [Route("getSerieSearchList")]
        [HttpGet]
        public async Task<IActionResult> GetSerieSearchListAsync([FromQuery] string filter, [FromQuery] int page, [FromQuery] string language, [FromQuery] int userId)
        {
            return Ok(await new SerieService(_dbContext).GetSerieSearchList(filter, page, language, userId));
        }

        // GET api/<SerieController>/getSeriePopularListt?page=1&language=es-ES
        [Route("getSeriePopularList")]
        [HttpGet]
        public async Task<IActionResult> GetSeriePopularListAsync([FromQuery] string language, [FromQuery] int userId)
        {
            return Ok(await new SerieService(_dbContext).GetSeriePopularList(language, userId));
        }

        // GET api/<SerieController>/getSerieTopRatedList?page=1&language=es-ES
        [Route("getSerieTopRatedList")]
        [HttpGet]
        public async Task<IActionResult> GetSerieTopRatedListAsync([FromQuery] string language, [FromQuery] int userId)
        {
            return Ok(await new SerieService(_dbContext).GetSerieTopRatedList(language, userId));
        }

        // GET api/<SerieController>?id=5&language=es-ES
        [Route("getSerie")]
        [HttpGet]
        public async Task<IActionResult> GetSerieAsync([FromQuery] int id, [FromQuery] string language, [FromQuery] int userId)
        {
            SerieService serieService = new SerieService(this._dbContext);

            return Ok(await serieService.CreateSerie(id, language, userId));
        }

        // GET api/<SerieController>?serieApiId=5&language=es-ES
        [Route("getSeasonsEpisodesList")]
        [HttpGet]
        public async Task<IActionResult> GetSeasonsEpisodesList([FromQuery] int serieApiId, [FromQuery] int userId)
        {
            SerieService serieService = new SerieService(this._dbContext);

            return Ok(await serieService.GetSeasonsEpisodesList(serieApiId, userId));
        }

        // GET api/<SerieController>/getReviews?serieApiId=5
        [Route("getReviews")]
        [HttpGet]
        public async Task<IActionResult> GetReviewsAsync([FromQuery] int serieApiId)
        {
            SerieService serieService = new SerieService(this._dbContext);

            return Ok(await serieService.GetSerieReviews(serieApiId));
        }

        // POST api/<SerieController>/createReview
        [Route("createReview")]
        [HttpPost]
        public async Task<IActionResult> CreateSerieReviewListAsync([FromBody] CreateSerieReviewListRequest request)
        {
            SerieService serieService = new SerieService(this._dbContext);

            return Ok(await serieService.CreateSerieReviewList(request));
        }

        // POST api/<SerieController>/setSerieRating?serieApiId=76600&userId=1&rating=5
        [Route("setSerieRating")]
        [HttpPost]
        public async Task<IActionResult> SetSerieRating([FromQuery] int serieApiId, [FromQuery] int userId, [FromQuery] int rating)
        {
            SerieService serieService = new SerieService(_dbContext);

            return Ok(await serieService.SetSerieRating(serieApiId, userId, rating));
        }

        // GET api/<SerieController>/getSerieRatings?userId=1&serieApiId=5
        [Route("getSerieRatings")]
        [HttpGet]
        public async Task<IActionResult> GetReviewsAsync([FromQuery] int userId, [FromQuery] int serieApiId)
        {
            SerieService serieService = new SerieService(this._dbContext);

            return Ok(await serieService.GetSerieRatings(userId, serieApiId));
        }

        // POST api/<SerieController>/setSerieWatched?serieApiId=76600&userId=1&watched=true
        [Route("setSerieWatched")]
        [HttpPost]
        public async Task<IActionResult> setSerieWatched([FromQuery] int serieApiId, [FromQuery] int userId, [FromQuery] string language, [FromQuery] bool watched)
        {
            SerieService serieService = new SerieService(_dbContext);

            return Ok(await serieService.setSerieWatched(serieApiId, userId, language, watched));
        }

        // POST api/<SeasonEpisodeController>/setSeasonEpisodeWatched?SeasonEpisode=76600&userId=1&watched=true
        [Route("setSeasonEpisodeWatched")]
        [HttpPost]
        public async Task<IActionResult> setSeasonEpisodeWatched([FromQuery] int SerieApiId, [FromQuery] int seasonsEpisodeId, [FromQuery] int userId, [FromQuery] bool watched)
        {
            SerieService serieService = new SerieService(_dbContext);

            return Ok(await serieService.setSeasonEpisodeWatched(SerieApiId, seasonsEpisodeId, userId, watched));
        }

        // POST api/<SerieController>/setSerieFavorite?serieApiId=76600&userId=1&favorite=true
        [Route("setSerieFavorite")]
        [HttpPost]
        public async Task<IActionResult> setSerieFavorite([FromQuery] int serieApiId, [FromQuery] int userId, [FromQuery] string language, [FromQuery] bool favorite)
        {
            SerieService serieService = new SerieService(_dbContext);

            return Ok(await serieService.setSerieFavorite(serieApiId, userId, language, favorite));
        }

    }
}
