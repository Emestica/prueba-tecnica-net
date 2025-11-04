namespace PruebaTecnica.Api.Domain.ADO.DTOS
{
    public class ProductosDTO
    {
        public int IdProducto { get; set; }
        public string Producto { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal PrecioProducto { get; set; }
        public int Existencias { get; set; }
        public string EstadoLegible { get; set; } = string.Empty;
        public bool? Estado { get; set; }
        public int IdUsuarioCreacion { get; set; }
        public string CreadoPor { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public int? IdUsuarioModificacion { get; set; }
        public string? ModificadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
