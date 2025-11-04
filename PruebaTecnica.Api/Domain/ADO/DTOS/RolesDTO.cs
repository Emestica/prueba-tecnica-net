namespace PruebaTecnica.Api.Domain.ADO.DTOS
{
    public class RolesDTO
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public string DescripcionRol { get; set; }
        public string EstadoLegible { get; set; }
        public bool EstadoRol { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
