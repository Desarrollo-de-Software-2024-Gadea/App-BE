using Microsoft.AspNetCore.Mvc;
using SerializedStalker.Series;
using System.Threading.Tasks;

namespace SerializedStalker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeriesController : ControllerBase
    {
        private readonly ISeriesApiService _seriesApiService;

        public SeriesController(ISeriesApiService seriesApiService)
        {
            _seriesApiService = seriesApiService;
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarSeriesAsync(string titulo, string genero)
        {
            var result = await _seriesApiService.BuscarSerieAsync(titulo, genero);
            return Ok(result);
        }
    }
}
