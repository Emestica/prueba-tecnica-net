using Microsoft.Data.SqlClient;
using PruebaTecnica.Api.Domain.ADO.DTOS;
using PruebaTecnica.Api.Domain.ADO.DTOS.Seguridad;
using PruebaTecnica.Api.Domain.ADO.Service;
using System.Data;

namespace PruebaTecnica.Api.Domain.ADO.Repository
{
    public class TransactionalRepository : ITransactionalRepository
    {
        private readonly IBaseDatosServicio _baseDatosServicio;
        private readonly ILogger<TransactionalRepository> _logger;

        public TransactionalRepository(IBaseDatosServicio baseDatosServicio, ILogger<TransactionalRepository> logger)
        {
            _baseDatosServicio = baseDatosServicio;
            _logger = logger;
        }

        public async Task<string> GuardarVenta(IEnumerable<RequestSales> dto)
        {
            _logger.LogInformation("GuardarVenta() => Iniciando");
            try
            {
                var dataTable = new DataTable();

                dataTable.Columns.Add("fk_producto", typeof(int));
                dataTable.Columns.Add("precio_unitario", typeof(decimal));
                dataTable.Columns.Add("cantidad_producto", typeof(int));
                
                int idUserCreation = 0;
                foreach (var item in dto)
                {
                    _logger.LogInformation("GuardarVenta() => Id Producto: {intIdProducto}, Cantidad: {intCantidad}, Precio Unitario: {decPrecioUnitario}",
                        item.IdProducto, item.Cantidad, item.PrecioUnitario);

                    dataTable.Rows.Add(item.IdProducto, item.PrecioUnitario, item.Cantidad);
                    idUserCreation = item.IdUsuario;
                }

                using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();

                await conexion.OpenAsync();

                using SqlCommand command = new SqlCommand("negocio.sp_crear_venta_producto", conexion)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@fk_usuario", SqlDbType.Int) { Value = idUserCreation });
                command.Parameters.Add(new SqlParameter("@listado_articulos", SqlDbType.Structured) { Value = dataTable });

                var outputParam = new SqlParameter("@resultado", SqlDbType.NVarChar, -1)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);
                int resExecNonQuery = await command.ExecuteNonQueryAsync();
                var resultado = outputParam.Value == DBNull.Value ? string.Empty : outputParam.Value.ToString() ?? string.Empty;

                _logger.LogInformation("GuardarVenta() => TB COMPAS - ResExecNonQuery: {intCode}, Resultado: {stringRes}", resExecNonQuery, resultado);
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GuardarVenta() => Exception");
                return "Error al guardar venta.";
            }
            finally
            {
                _logger.LogInformation("GuardarVenta() => Completado");
            }
        }

        public async Task<string> GuardarProducto(ProductosDTO dto)
        {
            _logger.LogInformation("GuardarProducto() => Iniciando");
            using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();
            using (SqlCommand command = new SqlCommand("negocio.sp_productos_crear", conexion))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@nombre_producto", dto.Producto);
                command.Parameters.AddWithValue("@descripcion_producto", dto.Descripcion);
                command.Parameters.AddWithValue("@precio_unitario", dto.PrecioProducto);
                command.Parameters.AddWithValue("@existencias", dto.Existencias);
                command.Parameters.AddWithValue("@fk_usuario_creacion", dto.IdUsuarioCreacion);

                SqlParameter outputParam = new SqlParameter("@resultado", System.Data.SqlDbType.NVarChar, -1)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);
                await conexion.OpenAsync();
                await command.ExecuteNonQueryAsync();
                string mensaje = outputParam.Value.ToString() ?? string.Empty;
                _logger.LogInformation("GuardarProducto() => Mensaje: ${}", mensaje);
                _logger.LogInformation("GuardarProducto() => Completado");
                return mensaje;
            }

        }

        public async Task<string> ActualizarPrecioProducto(int idProducto, decimal precioProducto, int idUserMod)
        {
            _logger.LogInformation("ActualizarPrecioProducto() => Iniciando");
            _logger.LogInformation("ActualizarPrecioProducto() => Id Producto: {}", idProducto);
            _logger.LogInformation("ActualizarPrecioProducto() => Precio Producto: {}", precioProducto);
            _logger.LogInformation("ActualizarPrecioProducto() => Id User Mod: {}", precioProducto);

            using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();
            using (SqlCommand command = new SqlCommand("negocio.sp_productos_actualizar", conexion))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@id_producto", idProducto);
                command.Parameters.AddWithValue("@nombre_producto", "");
                command.Parameters.AddWithValue("@descripcion_producto", "");
                command.Parameters.AddWithValue("@precio_unitario", precioProducto);
                command.Parameters.AddWithValue("@existencias", "");
                command.Parameters.AddWithValue("@fk_usuario_modificacion", idUserMod);

                SqlParameter outputParam = new SqlParameter("@resultado", System.Data.SqlDbType.NVarChar, -1)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);
                await conexion.OpenAsync();
                await command.ExecuteNonQueryAsync();
                string mensaje = outputParam.Value.ToString() ?? string.Empty;
                _logger.LogInformation("ActualizarPrecioProducto() => Mensaje: ${}", mensaje);
                _logger.LogInformation("ActualizarPrecioProducto() => Completado");
                return mensaje;
            }
        }
    }
}
