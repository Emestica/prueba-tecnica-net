namespace PruebaTecnica.Api.Domain.ADO.DTOS.Seguridad
{
    public class UserOptionOneDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool UserState { get; set; }
        public bool UserRolState { get; set; }
        public int UserRolId { get; set; }
        public int RolId { get; set; }
        public string Rol { get; set; } = string.Empty;
        public string RolDescription { get; set; } = string.Empty;
    }
}
