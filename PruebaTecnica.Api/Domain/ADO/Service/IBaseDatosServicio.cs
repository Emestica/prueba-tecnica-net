using Microsoft.Data.SqlClient;

namespace PruebaTecnica.Api.Domain.ADO.Service
{
    public interface IBaseDatosServicio
    {
        public SqlConnection ObtenerConexionParaADO();
    }
}
