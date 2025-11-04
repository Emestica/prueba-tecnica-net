using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PruebaTecnica.Api.Domain.ADO.DTOS;
using PruebaTecnica.Api.Domain.ADO.DTOS.Seguridad;
using PruebaTecnica.Api.Domain.ADO.Service;
using System.Data;
using System.Reflection.PortableExecutable;

namespace PruebaTecnica.Api.Domain.ADO.Repository
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly IBaseDatosServicio _baseDatosServicio;
        private readonly ILogger<SecurityRepository> _logger;

        public UserCredentialsDTO? UserCredentialsDTO { get; private set; }

        public SecurityRepository(IBaseDatosServicio baseDatosServicio, ILogger<SecurityRepository> logger)
        {
            this._baseDatosServicio = baseDatosServicio;
            this._logger = logger;
        }

        public async Task<UserOptionOneDTO?> ObtenerUsuario(string username, int opcion, int idUsuario)
        {
            _logger.LogInformation("ObtenerUsuario() => Iniciando");
            UserOptionOneDTO? resultado = null;
            try
            {
                using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();

                await conexion.OpenAsync();

                using SqlCommand command = new SqlCommand("seguridad.sp_obtener_usuario", conexion)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@opcion", opcion);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@id_usuario", idUsuario);

                using SqlDataReader reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow);

                if (await reader.ReadAsync())
                {
                    if (opcion == 1)
                    {
                        var user = new UserOptionOneDTO
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            UserState = reader.GetBoolean(reader.GetOrdinal("user_state")),
                            UserRolState = reader.GetBoolean(reader.GetOrdinal("user_rol_state")),
                            UserRolId = reader.GetInt32(reader.GetOrdinal("user_rol_id")),
                            RolId = reader.GetInt32(reader.GetOrdinal("rol_id")),
                            Rol = reader.GetString(reader.GetOrdinal("rol")),
                            RolDescription = reader.GetString(reader.GetOrdinal("rol_description"))
                        };
                        resultado = user;
                        var userCredentials = new UserCredentialsDTO
                        {
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Password = reader.GetString(reader.GetOrdinal("password"))
                        };
                        UserCredentialsDTO = userCredentials;
                    }
                }
                else
                {
                    _logger.LogWarning("ObtenerUsuario() => Usuario no encontrado");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerUsuario() => Exception");
            }
            finally
            {
                _logger.LogInformation("ObtenerUsuario() => Completado");
            }
            return resultado;
        }

        public async Task<string> GuardarUsuario(RegisterUserDTO dto)
        {
            _logger.LogInformation("GuardarUsuario() => Iniciando");
            try
            {
                using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();

                await conexion.OpenAsync();

                using SqlCommand command = new SqlCommand("seguridad.sp_crear_reg_usuario", conexion)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar, 30) { Value = dto.User });
                command.Parameters.Add(new SqlParameter("@contrasenia", SqlDbType.NVarChar, 500) { Value = dto.PassEncrypt });

                var outputParam = new SqlParameter("@resultado", SqlDbType.NVarChar, -1)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);
                int resExecNonQuery = await command.ExecuteNonQueryAsync();
                var resultado = outputParam.Value == DBNull.Value ? string.Empty : outputParam.Value.ToString() ?? string.Empty;
                
                _logger.LogInformation("GuardarUsuario() => TB USER - ResExecNonQuery: {intCode}, Resultado: {stringRes}", resExecNonQuery, resultado);
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GuardarUsuario() => Exception");
                return "Error al guardar el usuario.";
            }
            finally
            {
                _logger.LogInformation("GuardarUsuario() => Completado");
            }
        }

        public async Task<string> GuardarUsuarioRol(RegisterUserDTO dto)
        {
            _logger.LogInformation("GuardarUsuarioRol() => Iniciando");
            try
            {
                using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();

                await conexion.OpenAsync();

                using SqlCommand command = new SqlCommand("seguridad.sp_crear_reg_usuario_rol", conexion)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@id_usuario", SqlDbType.Int) { Value = dto.UserId });
                command.Parameters.Add(new SqlParameter("@id_rol", SqlDbType.Int) { Value = dto.RolId });

                var outputParam = new SqlParameter("@resultado", SqlDbType.NVarChar, -1)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);
                int resExecNonQuery = await command.ExecuteNonQueryAsync();
                var resultado = outputParam.Value == DBNull.Value ? string.Empty : outputParam.Value.ToString() ?? string.Empty;

                _logger.LogInformation("GuardarUsuarioRol() => TB USER ROL - ResExecNonQuery: {intCode}, Resultado: {stringRes}", resExecNonQuery, resultado);
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GuardarUsuarioRol() => Exception");
                return "Error al guardar el usuario rol.";
            }
            finally
            {
                _logger.LogInformation("GuardarUsuarioRol() => Completado");
            }
        }

        public async Task<IEnumerable<RolesDTO>> PruebaConsultarRoles(int opcion)
        {
            _logger.LogInformation("PruebaConsultarRoles() => Iniciando");
            _logger.LogInformation("PruebaConsultarRoles() => Opcion: ${}", opcion);

            var lista = new List<RolesDTO>();

            using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();
            await conexion.OpenAsync();
            using (SqlCommand command = new SqlCommand("seguridad.sp_mostrar_roles", conexion))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@opcion", opcion);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        RolesDTO rol = new RolesDTO
                        {
                            IdRol = reader.GetInt32(reader.GetOrdinal("id_rol")),
                            NombreRol = reader.GetString(reader.GetOrdinal("nombre_rol")),
                            DescripcionRol = reader.GetString(reader.GetOrdinal("descripcion_rol")),
                            FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                            FechaModificacion = reader.IsDBNull(reader.GetOrdinal("fecha_modificacion")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha_modificacion"))
                        };

                        if (opcion == 1)
                        {
                            rol.EstadoLegible = reader.GetString(reader.GetOrdinal("estado"));
                        }
                        else
                        {
                            rol.EstadoRol = reader.GetBoolean(reader.GetOrdinal("estado"));
                        }
                        lista.Add(rol);
                    }
                }
                _logger.LogInformation("PruebaConsultarRoles() => Completado");
                return lista;
            }
        }

        public async Task<string> PruebaGuardarRol(string nombreRol, string descripcionRol)
        {
            _logger.LogInformation("PruebaGuardarRol() => Iniciando");
            using SqlConnection conexion = _baseDatosServicio.ObtenerConexionParaADO();
            using (SqlCommand command = new SqlCommand("seguridad.sp_crear_reg_roles", conexion))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@nombre_rol", nombreRol);
                command.Parameters.AddWithValue("@descripcion_rol", descripcionRol);

                SqlParameter outputParam = new SqlParameter("@resultado", System.Data.SqlDbType.NVarChar, -1)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);
                await conexion.OpenAsync();
                await command.ExecuteNonQueryAsync();
                string mensaje = outputParam.Value.ToString() ?? string.Empty;
                _logger.LogInformation("PruebaGuardarRol() => Mensaje: ${}", mensaje);
                _logger.LogInformation("PruebaGuardarRol() => Completado");
                return mensaje;
            }

        }
    }
}
