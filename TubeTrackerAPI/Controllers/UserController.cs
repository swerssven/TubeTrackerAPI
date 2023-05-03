using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using TubeTrackerAPI.Middleware;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            UserService userService = new UserService(_tubeTrackerDbContext);
            UserDto userResponse = await userService.GetUser(id);

            return Ok(userResponse);
        }

        // POST api/<UserController>
        [HttpPost]
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
        [Route("GetUserStatistics")]
        [HttpGet]
        public async Task<IActionResult> GetUserStatistics([FromQuery] int userId)
        {
            UserService userService = new UserService(_tubeTrackerDbContext);
            UserStatisticsDto userStatisticsDto = await userService.GetUserStatistics(userId);

            return Ok(userStatisticsDto);
        }
    }
}
