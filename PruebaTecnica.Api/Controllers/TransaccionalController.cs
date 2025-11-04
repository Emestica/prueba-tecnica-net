using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Api.Domain.ADO.DTOS;
using PruebaTecnica.Api.Domain.ADO.Repository;

namespace PruebaTecnica.Api.Controllers
{
    [ApiController]
    [Route("api/transactional")]
    public class TransaccionalController : ControllerBase
    {
        private readonly ILogger<TransaccionalController> _logger;
        private readonly ITransactionalRepository _transaccionalRepositorio;

        public TransaccionalController(ILogger<TransaccionalController> logger, ITransactionalRepository transaccionalRepositorio)
        {
            this._logger = logger;
            this._transaccionalRepositorio = transaccionalRepositorio;
        }

        [HttpPost("save-product")]
        public async Task<ActionResult> GuardarProducto(ProductosDTO dto)
        {
            _logger.LogInformation("GuardarProducto() => Iniciando");
            try
            {
                var resultado = await _transaccionalRepositorio.GuardarProducto(dto);
                return Ok(new { response = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GuardarProducto() => Exception");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
            finally
            {
                _logger.LogInformation("GuardarProducto() => Completado");
            }
        }

        [HttpPost("save-sale")]
        public async Task<ActionResult> GuardarVenta(IEnumerable<RequestSales> requestSales)
        {
            _logger.LogInformation("GuardarProducto() => Iniciando");
            try
            {
                var resultado = await _transaccionalRepositorio.GuardarVenta(requestSales);
                return Ok(new { response = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GuardarProducto() => Exception");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
            finally
            {
                _logger.LogInformation("GuardarProducto() => Completado");
            }
        }

        [HttpPost("update-price-product")]
        public async Task<ActionResult> ActualizarPrecioProducto(int idProducto, int idUsuario, decimal precio)
        {
            _logger.LogInformation("ActualizarPrecioProducto() => Iniciando");
            try
            {
                var resultado = await _transaccionalRepositorio.ActualizarPrecioProducto(idProducto, precio, idUsuario);
                return Ok(new { response = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActualizarPrecioProducto() => Exception");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
            finally
            {
                _logger.LogInformation("ActualizarPrecioProducto() => Completado");
            }
        }
    }
}
