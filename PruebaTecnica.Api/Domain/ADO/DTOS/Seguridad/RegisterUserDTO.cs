using System.Text.Json.Serialization;

namespace PruebaTecnica.Api.Domain.ADO.DTOS.Seguridad
{
    public class RegisterUserDTO
    {
        [JsonPropertyName("username")]
        public string User { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Pass { get; set; } = string.Empty;

        [JsonIgnore]
        public string PassEncrypt { get; set; } = string.Empty;

        [JsonPropertyName("rol_id")]
        public int RolId { get; set; }

        [JsonIgnore]
        public int? UserId { get; set; }
    }
}
