using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;

namespace TubeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TubeTrackerDbContext _dbContext;

        public SocialController(IConfiguration configuration, TubeTrackerDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        //GET api/<SocialController>/getPosts/
        [Route("getPostsList")]
        [HttpGet]
        public async Task<IActionResult> GetPostsListAsync([FromQuery] bool forFriends, [FromQuery] int userId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.GetPostsList(forFriends, userId));
        }

        //POST api/<SocialController>/createPost
        [Route("createPost")]
        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostRequest request)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.CreatePost(request));
        }
    }
}
