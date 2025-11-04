using Microsoft.Data.SqlClient;
using PruebaTecnica.Api.Domain.ADO.DTOS;
using PruebaTecnica.Api.Domain.ADO.DTOS.Seguridad;
using PruebaTecnica.Api.Domain.ADO.Service;

namespace PruebaTecnica.Api.Domain.ADO.Repository
{
    public class ConsultingRepository : IConsultingRepository
    {
        private readonly IBaseDatosServicio _baseDatosServicio;
        private readonly ILogger<ConsultingRepository> _logger;
        public UserCredentialsDTO? UserCredentialsDTO { get; private set; }

        public ConsultingRepository(IBaseDatosServicio baseDatosServicio, ILogger<ConsultingRepository> logger)
        {
            this._baseDatosServicio = baseDatosServicio;
            this._logger = logger;
        }

        public async Task<IEnumerable<ProductosDTO>> PruebaConsultarProductos(int opcion)
        {
            _logger.LogInformation("PruebaConsultarProductos() => Iniciando");
            _logger.LogInformation("PruebaConsultarProductos() => Opcion: {}", opcion);

            var lista = new List<ProductosDTO>();

            using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();
            await conexion.OpenAsync();
            using (SqlCommand command = new SqlCommand("negocio.sp_mostrar_productos", conexion))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@opcion", opcion);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ProductosDTO dto = new ProductosDTO();

                        dto.IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto"));
                        dto.Producto = reader.GetString(reader.GetOrdinal("producto"));
                        dto.Descripcion = reader.GetString(reader.GetOrdinal("descripcion"));
                        dto.PrecioProducto = reader.GetDecimal(reader.GetOrdinal("precio_producto"));
                        dto.Existencias = reader.GetInt32(reader.GetOrdinal("existencias"));
                        if (opcion == 1)
                        {
                            dto.EstadoLegible = reader.GetString(reader.GetOrdinal("estado"));
                        }
                        else
                        {
                            dto.Estado = reader.GetBoolean(reader.GetOrdinal("estado"));
                        }
                        dto.IdUsuarioCreacion = reader.GetInt32(reader.GetOrdinal("id_usuario_creacion"));
                        dto.CreadoPor = reader.GetString(reader.GetOrdinal("creado_por"));
                        dto.FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion"));
                        dto.ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por")) ? null : reader.GetString(reader.GetOrdinal("modificado_por"));
                        dto.FechaModificacion = reader.IsDBNull(reader.GetOrdinal("fecha_modificacion")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha_modificacion"));
                        
                        lista.Add(dto);
                    }
                }
                _logger.LogInformation("PruebaConsultarProductos() => Completado");
                return lista;
            }
        }

        public async Task<IEnumerable<HistoricoPrecioProductosDTO>> PruebaConsultarHistoricoPrecioProductos(int opcion)
        {
            _logger.LogInformation("PruebaConsultarHistoricoPrecioProductos() => Iniciando");
            _logger.LogInformation("PruebaConsultarHistoricoPrecioProductos() => Opcion: {}", opcion);

            var lista = new List<HistoricoPrecioProductosDTO>();

            using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();
            await conexion.OpenAsync();
            using (SqlCommand command = new SqlCommand("negocio.sp_mostrar_historico_precio_productos", conexion))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@opcion", opcion);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        HistoricoPrecioProductosDTO dto = new HistoricoPrecioProductosDTO();

                        dto.IdHistorico = reader.GetInt32(reader.GetOrdinal("id_historico"));
                        dto.IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto"));
                        dto.Producto = reader.GetString(reader.GetOrdinal("producto"));
                        dto.Descripcion = reader.GetString(reader.GetOrdinal("descripcion"));
                        dto.IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario"));
                        dto.ActualizadoPor = reader.GetString(reader.GetOrdinal("actualizado_por"));
                        dto.PrecioNuevo = reader.GetDecimal(reader.GetOrdinal("precio_nuevo"));
                        dto.PrecioAntiguo = reader.GetDecimal(reader.GetOrdinal("precio_antiguo"));
                        dto.FechaModificacion = reader.GetDateTime(reader.GetOrdinal("fecha_modificacion"));

                        lista.Add(dto);
                    }
                }
                _logger.LogInformation("PruebaConsultarHistoricoPrecioProductos() => Completado");
                return lista;
            }
        }
    }
}
