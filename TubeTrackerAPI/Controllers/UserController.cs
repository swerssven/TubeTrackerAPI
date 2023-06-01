using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using TubeTrackerAPI.Middleware;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TubeTrackerDbContext _tubeTrackerDbContext;

        public UserController(TubeTrackerDbContext tubeTrackerDbContext) 
        {
            _tubeTrackerDbContext = tubeTrackerDbContext;
        }

        // GET api/<UserController>/5
        [Middleware.Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            UserService userService = new UserService(_tubeTrackerDbContext);
            UserDto userResponse = await userService.GetUser(id);

            return Ok(userResponse);
        }

        // GET api/<UserController>/GetUserList
        [Middleware.Authorize]
        [HttpGet]
        [Route("GetUserList")]
        public async Task<IActionResult> GetUserList()
        {
            UserService userService = new UserService(_tubeTrackerDbContext);
            List<usersGridDto> userResponse = await userService.GetUserList();

            return Ok(userResponse);
        }

        // POST api/<UserController>
        [AllowAnonymous]
        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            BaseResponse response = await new UserService(_tubeTrackerDbContext).CreateUser(user);
            if (response.Status == StatusEnum.Ok)
            {
                return NoContent();
            }
            else if (response.Status == StatusEnum.EmailAlreadyExists || 
                     response.Status == StatusEnum.NickNameAlreadyExists || 
                     response.Status == StatusEnum.EmailAndNicknameAlreadyExist)
            {
                return BadRequest(response.Status.ToString());
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }
        }

        // GET api/<UserController>/GetUserStatistics?userId=3
        [Middleware.Authorize]
        [Route("GetUserStatistics")]
        [HttpGet]
        public async Task<IActionResult> GetUserStatistics([FromQuery] int userId)
        {
            UserService userService = new UserService(_tubeTrackerDbContext);
            UserStatisticsDto userStatisticsDto = await userService.GetUserStatistics(userId);

            return Ok(userStatisticsDto);
        }

        // POST api/<UserController>
        [Middleware.Authorize]
        [HttpPost]
        [Route("EditUser")]
        public async Task<IActionResult> EditUser([FromBody] EditUserRequest user)
        {
            EditUserResponse response = await new UserService(_tubeTrackerDbContext).EditUser(user);
            if (response.Status == StatusEnum.Ok)
            {
                return Ok(response.user);
            }
            else if (response.Status == StatusEnum.EmailAlreadyExists ||
                     response.Status == StatusEnum.NickNameAlreadyExists ||
                     response.Status == StatusEnum.EmailAndNicknameAlreadyExist)
            {
                return BadRequest(response.Status.ToString());
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }
        }

        // POST api/<UserController>/MakeUserAdmin?userId=1&isAdmin=true
        [Middleware.Authorize]
        [HttpPost]
        [Route("MakeUserAdmin")]
        public async Task<IActionResult> MakeUserAdmin([FromQuery] int userId, [FromQuery] bool isAdmin)
        {
            UserService userService = new UserService(_tubeTrackerDbContext);
            bool response = await userService.MakeUserAdmin(userId, isAdmin);

            return Ok(response);
        }

        // POST api/<UserController>/ChangeUserState?userId=1&isActive=false
        [Middleware.Authorize]
        [HttpPost]
        [Route("ChangeUserState")]
        public async Task<IActionResult> ChangeUserState([FromQuery] int userId, [FromQuery] bool isActive)
        {
            UserService userService = new UserService(_tubeTrackerDbContext);
            bool response = await userService.ChangeUserState(userId, isActive);

            return Ok(response);
        }

        //DELETE api/<UserController>/DeleteUser?userId=3
        [Middleware.Authorize]
        [Route("DeleteUser")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromQuery] int userId)
        {
            UserService userService = new UserService(_tubeTrackerDbContext);

            return Ok(await userService.DeleteUser(userId));
        }
    }
}
