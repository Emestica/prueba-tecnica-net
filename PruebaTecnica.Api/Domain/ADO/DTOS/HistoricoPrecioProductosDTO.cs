namespace PruebaTecnica.Api.Domain.ADO.DTOS
{
    public class HistoricoPrecioProductosDTO
    {
        public int IdHistorico { get; set; }
        public int IdProducto { get; set; }
        public string Producto { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int IdUsuario { get; set; }
        public string ActualizadoPor { get; set; } = string.Empty;
        public decimal PrecioNuevo { get; set; }
        public decimal PrecioAntiguo { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
