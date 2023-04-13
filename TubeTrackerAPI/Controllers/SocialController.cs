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

        //GET api/<SocialController>/getFriends?searchParam=carlitos
        [Route("getSearchFriendsList")]
        [HttpGet]
        public async Task<IActionResult> getSearchFriendsListAsync([FromQuery] int userId, [FromQuery] string searchParam)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.GetSearchFriendsList(userId, searchParam));
        }

        //GET api/<SocialController>/getFriends?userId=1
        [Route("getFriendsList")]
        [HttpGet]
        public async Task<IActionResult> GetFriendsListAsync([FromQuery] int userId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.GetFriendsList(userId));
        }

        //POST api/<SocialController>/createFriendInvitation?userId=1&friendUserId=2
        [Route("createFriendInvitation")]
        [HttpPost]
        public async Task<IActionResult> CreateFriendInvitationAsync([FromQuery] int userId, [FromQuery] int friendUserId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.CreateFriendInvitation(userId, friendUserId));
        }

        //POST api/<SocialController>/AcceptFriendship?userId=1&friendUserId=2
        [Route("acceptFriendship")]
        [HttpPost]
        public async Task<IActionResult> AcceptFriendship([FromQuery] int userId, [FromQuery] int friendUserId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.AcceptFriendship(userId, friendUserId));
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
