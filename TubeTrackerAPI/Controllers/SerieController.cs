using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;

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

        // GET api/<SerieController>
        [Route("getSerieList")]
        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery] string filter, [FromQuery] int page, [FromQuery] string language)
        {
            return Ok(await new SerieService(_dbContext).GetSerieSearchList(filter, page, language));
        }

        // GET api/<SerieController>?id=5&language=es-ES
        [Route("getSerie")]
        [HttpGet]
        public async Task<IActionResult> GetSerieAsync([FromQuery] int id, [FromQuery] string language)
        {
            SerieService serieService = new SerieService(this._dbContext);

            return Ok(await serieService.CreateSerie(id, language));
        }
    }
}
