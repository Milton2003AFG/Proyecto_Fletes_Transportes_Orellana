using Transportes_Orellana.Models;
using Transportes_Orellana.Models.ViewModels;

namespace Transportes_Orellana.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> ObtenerResumenGeneralAsync();
}