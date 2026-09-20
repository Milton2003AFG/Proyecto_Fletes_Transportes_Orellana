using Transportes_Orellana.Models.ViewModels;

namespace Transportes_Orellana.Services.Interfaces;

public interface IReporteUtilidadesPdfService
{
    byte[] GenerarReporte(CalculoUtilidadViewModel utilidad);
}