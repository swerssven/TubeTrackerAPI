using Microsoft.AspNetCore.Mvc;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;

namespace TubeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TubeTrackerDbContext  _dbContext;

        public AuthController(IConfiguration configuration, TubeTrackerDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] GetTokenRequest request)
        {
            AuthService authService = new AuthService(_configuration, _dbContext);
            GetTokenResponse response = await authService.GetToken(request);

            if (response.Status == StatusEnum.Ok)
            {
                return Ok(response.Token);
            }
            else if(response.Status == StatusEnum.NotFound)
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
