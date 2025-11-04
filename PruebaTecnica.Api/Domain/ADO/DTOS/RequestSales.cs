using System.Text.Json.Serialization;

namespace PruebaTecnica.Api.Domain.ADO.DTOS
{
    public class RequestSales
    {
        [JsonPropertyName("product_id")]
        public int IdProducto { get; set; }
        [JsonPropertyName("quantity")]
        public int Cantidad { get; set; }
        [JsonPropertyName("unit_price")]
        public decimal PrecioUnitario { get; set; }
        [JsonPropertyName("user_at")]
        public int IdUsuario { get; set; }
    }
}
