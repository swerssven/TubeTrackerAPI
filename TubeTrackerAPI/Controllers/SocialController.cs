using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TubeTrackerAPI.Middleware;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;

namespace TubeTrackerAPI.Controllers
{
    [Authorize]
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
        [Route("friends/getSearchFriendsList")]
        [HttpGet]
        public async Task<IActionResult> getSearchFriendsListAsync([FromQuery] int userId, [FromQuery] string searchParam)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.GetSearchFriendsList(userId, searchParam));
        }

        //GET api/<SocialController>/getFriends?userId=1
        [Route("friends/getFriendsList")]
        [HttpGet]
        public async Task<IActionResult> GetFriendsListAsync([FromQuery] int userId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.GetFriendsList(userId));
        }

        //GET api/<SocialController>/getFriendsWithMessagesList?userId=1
        [Route("friends/getFriendsWithMessagesList")]
        [HttpGet]
        public async Task<IActionResult> GetFriendsWithMessagesListAsync([FromQuery] int userId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.GetFriendsWithMessagesList(userId));
        }

        //POST api/<SocialController>/createFriendInvitation?userId=1&friendUserId=2
        [Route("friends/createFriendInvitation")]
        [HttpPost]
        public async Task<IActionResult> CreateFriendInvitationAsync([FromQuery] int userId, [FromQuery] int friendUserId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.CreateFriendInvitation(userId, friendUserId));
        }

        //POST api/<SocialController>/AcceptFriendship?userId=1&friendUserId=2
        [Route("friends/acceptFriendship")]
        [HttpPost]
        public async Task<IActionResult> AcceptFriendshipAsync([FromQuery] int userId, [FromQuery] int friendUserId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.AcceptFriendship(userId, friendUserId));
        }

        //POST api/<SocialController>/createMessage
        [Route("messages/createMessage")]
        [HttpPost]
        public async Task<IActionResult> CreateMessagesAsync([FromBody] CreateMessageRequest request)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.CreateMessage(request));
        }

        //GET api/<SocialController>/getMessageList?userId=1&friendUserId=2
        [Route("messages/getMessagesList")]
        [HttpGet]
        public async Task<IActionResult> GetMessagesListAsync(int userId, int friendUserId)
        {
            SocialService socialService = new SocialService(_dbContext);

            return Ok(await socialService.getMessagesList(userId, friendUserId));
        }

        //GET api/<SocialController>/getNumberUnreadMessages?userId=1
        [Route("messages/getNumberUnreadMessages")]
        [HttpGet]
        public async Task<IActionResult> getNumberUnreadMessages(int userId)
        {
            SocialService socialService = new SocialService(_dbContext);

            return Ok(await socialService.getNumberUnreadMessages(userId));
        }

        //GET api/<SocialController>/getPosts/
        [Route("posts/getPostsList")]
        [HttpGet]
        public async Task<IActionResult> GetPostsListAsync([FromQuery] bool forFriends, [FromQuery] int userId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.GetPostsList(forFriends, userId));
        }

        //POST api/<SocialController>/createPost
        [Route("posts/createPost")]
        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostRequest request)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.CreatePost(request));
        }

        //POST api/<SocialController>/createPostComment
        [Route("posts/createPostComment")]
        [HttpPost]
        public async Task<IActionResult> CreatePostCommentAsync([FromBody] CreatePostCommentRequest request)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.CreatePostComment(request));
        }

        //GET api/<SocialController>/getCommentsList?userId=1&friendUserId=2
        [Route("posts/getCommentsList")]
        [HttpGet]
        public async Task<IActionResult> getCommentsList(int postId)
        {
            SocialService socialService = new SocialService(_dbContext);

            return Ok(await socialService.getCommentsList(postId));
        }

        //POST api/<SocialController>/createPostLike?userId=1&postId=3&liked=true.
        [Route("posts/createPostLike")]
        [HttpPost]
        public async Task<IActionResult> createPostLike([FromQuery] int userId, [FromQuery] int postId, [FromQuery] bool liked)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.createPostLike(userId, postId, liked));
        }

        //DELETE api/<SocialController>/deletePost?postId=3
        [Route("posts/deletePost")]
        [HttpPost]
        public async Task<IActionResult> deletePost([FromQuery] int postId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.deletePost(postId));
        }

        //DELETE api/<SocialController>/deletePostComment?postCommnentsId=3
        [Route("posts/deletePostComment")]
        [HttpPost]
        public async Task<IActionResult> deletePostComment([FromQuery] int postCommnentsId)
        {
            SocialService socialService = new SocialService(this._dbContext);

            return Ok(await socialService.deletePostComment(postCommnentsId));
        }
    }
}
