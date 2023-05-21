using Microsoft.AspNetCore.Mvc;
using TubeTrackerAPI.Middleware;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly TubeTrackerDbContext _dbContext;

        public NewsController(TubeTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // POST api/<NewsController>/createNewsArticle
        [Route("news/createNewsArticle")]
        [HttpPost]
        public async Task<IActionResult> CreateNewsArticle([FromBody] CreateNewsRequest newsRequest)
        {
            NewsService newsService = new NewsService(_dbContext);

            return Ok(await newsService.CreateNewsArticle(newsRequest));
        }

        // GET api/<NewsController>/getNewsArticles
        [Route("news/getNewsArticles")]
        [HttpGet]
        public async Task<IActionResult> GetNewsArticles()
        {
            NewsService newsService = new NewsService(_dbContext);

            return Ok(await newsService.GetNewsArticlesList());
        }
    }
}

