using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Api.Domain.ADO.DTOS.Seguridad;
using PruebaTecnica.Api.Domain.ADO.Repository;
using PruebaTecnica.Api.Services;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PruebaTecnica.Api.Controllers
{
    [ApiController]
    [Route("api/security")]
    public class SeguridadController : ControllerBase
    {
        private readonly ISecurityServices _securityServices;
        private readonly ISecurityRepository _securityRepository;
        private readonly ILogger<SeguridadController> _logger;

        public SeguridadController(ISecurityRepository securityRepository, 
            ILogger<SeguridadController> logger,
            ISecurityServices securityServices)
        {
            _securityRepository = securityRepository;
            _logger = logger;
            _securityServices = securityServices;
        }

        [HttpGet("login-user")]
        public async Task<IActionResult> Login(string username, string password)
        {
            _logger.LogInformation("GetUser() => Iniciando");
            try
            {
                var result = await _securityRepository.ObtenerUsuario(username, 1, 0);

                var credentials = _securityRepository.UserCredentialsDTO;
                if (result is null && credentials is null)
                {
                    _logger.LogWarning("GetUser() => Usuario invalido: {Username}", username);
                    return Unauthorized(new { Message = "Usuario invalido.", Result = result, Creden = credentials });
                }
                else
                {
                    if (_securityServices.VerifyPassword(credentials.Password, password))
                    {
                        _logger.LogInformation("GetUser() => Usuario autenticado correctamente: {Username}", username);
                        return Ok(new { data = result });
                    }
                    else 
                    { 
                        _logger.LogWarning("GetUser() => Contraseña invalida para el usuario: {Username}", username);
                        return Unauthorized(new { Message = "Contraseña invalida." });
                    }
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Error Interno.");
            }
            finally
            {
                _logger.LogInformation("GetUser() => Completado");
            }
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            _logger.LogInformation("RegisterUser() => Iniciando");
            try
            {
                var result = _securityServices.CreateCredentials(registerUserDTO.Pass);
                if (result is not null)
                {
                    registerUserDTO.PassEncrypt = result;
                    var resultCreateUser = await _securityRepository.GuardarUsuario(registerUserDTO);
                    if (resultCreateUser is not null)
                    {
                        using JsonDocument doc = JsonDocument.Parse(resultCreateUser);
                        JsonElement root = doc.RootElement;

                        if (root.TryGetProperty("data", out JsonElement dataElement))
                        {

                            int code = dataElement.GetProperty("code").GetInt32();
                            int id = dataElement.GetProperty("id_usuario").GetInt32();
                            string msg = dataElement.GetProperty("msg").GetString() ?? string.Empty;

                            _logger.LogInformation("RegisterUser() => TB - Code: {code}, Id Usuario: {id}, Msg: {msg}", code, id, msg);

                            registerUserDTO.UserId = id;
                            var resultCreateUserRol = await _securityRepository.GuardarUsuarioRol(registerUserDTO);
                            if (resultCreateUserRol is not null)
                            {
                                return Ok(new { code = 0, message = "Usuario registrado correctamente." });
                            }
                            else
                            {
                                return Ok(new { code = 1, error = "Error al asignar rol al usuario." });
                            }
                        }
                        else
                        {
                            return Ok(new { code = 1, error = "Error al obtener ID de usuario creado." });
                        }
                    }
                }
                else 
                {
                    return Ok(new { code = 1, error = "Error Fallo Encriptacion Contraseña." });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Error Interno.");
            }
            finally
            {
                _logger.LogInformation("RegisterUser() => Completado");
            }
            return Ok();
        }

        [HttpGet("test-get-all-roles/{opcion}")]
        public async Task<IActionResult> PruebaGetAllRoles(int opcion)
        {
            _logger.LogInformation("PruebaGetAllRoles() => Iniciando");
            try
            {
                var consultas = await _securityRepository.PruebaConsultarRoles(opcion);
                return Ok(consultas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PruebaGetAllRoles() => Exception");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
            finally
            {
                _logger.LogInformation("PruebaGetAllRoles() => Completado");
            }
        }

        [HttpPost("test-save-role/{rolName}/{rolDescription}")]
        public async Task<ActionResult> PruebaGuardarRol(string rolName, string rolDescription)
        {
            _logger.LogInformation("PruebaGuardarRol() => Iniciando");
            try
            {
                var resultado = await _securityRepository.PruebaGuardarRol(rolName, rolDescription);
                return Ok(new { response = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PruebaGuardarRol() => Exception");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
            finally
            {
                _logger.LogInformation("PruebaGuardarRol() => Completado");
            }
        }
    }
}
