using Microsoft.AspNetCore.Mvc;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Services;

namespace TubeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] GetTokenRequest request)
        {
            AuthService authService = new AuthService(_configuration);
            GetTokenResponse response = await authService.GetToken(request);

            if (response.Status == StatusEnum.Ok)
            {
                return Ok(response.Token);
            }
            else if(response.Status == StatusEnum.UserNotFound)
            {
                return Unauthorized();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }
        }

    }
}
