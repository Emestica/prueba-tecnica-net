using PruebaTecnica.Api.Domain.ADO.DTOS;

namespace PruebaTecnica.Api.Domain.ADO.Repository
{
    public interface ITransactionalRepository
    {
        /** METODOS OBLIGATORIOS */
        Task<string> GuardarVenta(IEnumerable<RequestSales> dto);
        Task<string> GuardarProducto(ProductosDTO dto);
        Task<string> ActualizarPrecioProducto(int idProducto, decimal precioProducto, int idUserMod);
    }
}
