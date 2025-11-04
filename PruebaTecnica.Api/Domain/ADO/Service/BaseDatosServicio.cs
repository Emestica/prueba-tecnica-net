using Microsoft.Data.SqlClient;

namespace PruebaTecnica.Api.Domain.ADO.Service
{
    public class BaseDatosServicio : IBaseDatosServicio
    {
        private readonly string _connectionString;

        public BaseDatosServicio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MicrosoftSQLConnection");
        }

        public SqlConnection ObtenerConexionParaADO()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
