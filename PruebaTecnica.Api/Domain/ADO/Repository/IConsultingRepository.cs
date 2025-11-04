using PruebaTecnica.Api.Domain.ADO.DTOS;
using PruebaTecnica.Api.Domain.ADO.DTOS.Seguridad;

namespace PruebaTecnica.Api.Domain.ADO.Repository
{
    public interface IConsultingRepository
    {
        Task<IEnumerable<ProductosDTO>> PruebaConsultarProductos(int opcion);
        Task<IEnumerable<HistoricoPrecioProductosDTO>> PruebaConsultarHistoricoPrecioProductos(int opcion);
    }
}
