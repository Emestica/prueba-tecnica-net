using PruebaTecnica.Api.Domain.ADO.DTOS;
using PruebaTecnica.Api.Domain.ADO.DTOS.Seguridad;

namespace PruebaTecnica.Api.Domain.ADO.Repository
{
    public interface ISecurityRepository
    {
        Task<UserOptionOneDTO?> ObtenerUsuario(string username, int opcion, int idUsuario);
        UserCredentialsDTO? UserCredentialsDTO { get; }
        Task<string> GuardarUsuario(RegisterUserDTO dto);
        Task<string> GuardarUsuarioRol(RegisterUserDTO dto);

        /** METODOS DE PRUEBAS */
        Task<IEnumerable<RolesDTO>> PruebaConsultarRoles(int opcion);
        Task<string> PruebaGuardarRol(string nombreRol, string descripcionRol);
    }
}
