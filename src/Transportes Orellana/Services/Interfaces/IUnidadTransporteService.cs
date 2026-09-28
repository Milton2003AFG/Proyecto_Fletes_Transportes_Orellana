using Transportes_Orellana.Models;

namespace Transportes_Orellana.Services.Interfaces
{
    public interface IUnidadTransporteService
    {
        Task<IEnumerable<UnidadTransporte>> ObtenerTodosAsync();

        Task<UnidadTransporte?> ObtenerPorIdAsync(int id);

        Task<(bool exito, string? error)> CrearAsync(
            UnidadTransporte unidad);

        Task<(bool exito, string? error)> ActualizarAsync(
            UnidadTransporte unidad);

        Task<bool> CambiarEstadoAsync(
            int id,
            string estado);
    }
}