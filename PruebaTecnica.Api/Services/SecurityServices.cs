using System.Security.Cryptography;

namespace PruebaTecnica.Api.Services
{
    public class SecurityServices : ISecurityServices
    {
        private readonly ILogger<SecurityServices> _logger;

        public SecurityServices(ILogger<SecurityServices> logger)
        {
            _logger = logger;
        }

        public string? CreateCredentials(string password)
        {
            _logger.LogInformation("CreateCredentials() => Iniciando");
            try
            {
                var passwordHash = HashPassword(password);

                var newObject = new
                {
                    Contrasenia = password,
                    Hash = passwordHash,
                    Verificacion = VerifyPassword(passwordHash, password)
                };

                return passwordHash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateCredentials() => Exception");
                return null;
            }
            finally
            {
                _logger.LogInformation("CreateCredentials() => Completado");
            }
        }

        // Genera un hash con salt usando PBKDF2 (SHA256)
        public string HashPassword(string password, int iterations = 100_000)
        {
            // Generar un salt aleatorio de 16 bytes
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            // Derivar la clave (hash)
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32); // 32 bytes = 256 bits

            // Guardar todo junto (para almacenar en BD)
            string result = $"PBKDF2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
            return result;
        }

        // Verifica una contraseña ingresada contra el hash almacenado
        public bool VerifyPassword(string passwordEncript, string password)
        {
            var parts = passwordEncript.Split('$', 4);
            if (parts.Length != 4 || parts[0] != "PBKDF2") return false;

            int iterations = int.Parse(parts[1]);
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] originalHash = Convert.FromBase64String(parts[3]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] testHash = pbkdf2.GetBytes(originalHash.Length);

            // Comparación segura en tiempo constante
            return CryptographicOperations.FixedTimeEquals(originalHash, testHash);
        }
    }
}
