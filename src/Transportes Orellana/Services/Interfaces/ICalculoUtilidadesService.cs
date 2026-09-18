using Transportes_Orellana.Models.ViewModels;

namespace Transportes_Orellana.Services.Interfaces;

public interface ICalculoUtilidadesService
{
    Task<IEnumerable<CalculoUtilidadViewModel>> ObtenerTodosAsync();

    Task<CalculoUtilidadViewModel?> ObtenerPorFleteAsync(int fleteId);
}