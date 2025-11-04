using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Api.Domain.ADO.Repository;

namespace PruebaTecnica.Api.Controllers
{
    [ApiController]
    [Route("api/consulting")]
    public class ConsultasController : ControllerBase
    {
        private readonly ILogger<ConsultasController> _logger;
        private readonly IConsultingRepository _consultasRepositorio;

        public ConsultasController(ILogger<ConsultasController> logger, IConsultingRepository consultasRepositorio)
        {
            this._logger = logger;
            this._consultasRepositorio = consultasRepositorio;
        }

        [HttpGet("test-get-all-products/{opcion}")]
        public async Task<IActionResult> PruebaGetAllProducts(int opcion)
        {
            _logger.LogInformation("PruebaGetAllProducts() => Iniciando");
            try
            {
                var consultas = await _consultasRepositorio.PruebaConsultarProductos(opcion);
                return Ok(consultas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PruebaGetAllProducts() => Exception");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
            finally
            {
                _logger.LogInformation("PruebaGetAllProducts() => Completado");
            }
        }

        [HttpGet("test-get-all-history-price-products/{opcion}")]
        public async Task<IActionResult> PruebaGetAllHistoryPriceProducts(int opcion)
        {
            _logger.LogInformation("PruebaGetAllHistoryPriceProducts() => Iniciando");
            try
            {
                var consultas = await _consultasRepositorio.PruebaConsultarHistoricoPrecioProductos(opcion);
                return Ok(consultas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PruebaGetAllHistoryPriceProducts() => Exception");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
            finally
            {
                _logger.LogInformation("PruebaGetAllHistoryPriceProducts() => Completado");
            }
        }
    }
}
