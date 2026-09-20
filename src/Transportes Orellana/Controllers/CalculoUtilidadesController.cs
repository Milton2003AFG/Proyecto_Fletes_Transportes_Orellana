using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Controllers;

[Authorize(Roles = "Admin,Motorista")]
public class CalculoUtilidadesController : Controller
{
    private readonly ICalculoUtilidadesService _calculoUtilidadesService;
    private readonly IReporteUtilidadesPdfService _reportePdfService;

    public CalculoUtilidadesController(
        ICalculoUtilidadesService calculoUtilidadesService,
        IReporteUtilidadesPdfService reportePdfService)
    {
        _calculoUtilidadesService = calculoUtilidadesService;
        _reportePdfService = reportePdfService;
    }

    public async Task<IActionResult> Index()
    {
        var utilidades =
            await _calculoUtilidadesService.ObtenerTodosAsync();

        return View(utilidades);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var utilidad =
            await _calculoUtilidadesService.ObtenerPorFleteAsync(id.Value);

        if (utilidad == null)
        {
            return NotFound();
        }

        return View(utilidad);
    }

    public async Task<IActionResult> DescargarPdf(int id)
    {
        var utilidad =
            await _calculoUtilidadesService.ObtenerPorFleteAsync(id);

        if (utilidad == null)
        {
            return NotFound();
        }

        var pdf = _reportePdfService.GenerarReporte(utilidad);

        var nombreArchivo =
            $"Utilidad_Flete_{utilidad.FleteId:D3}.pdf";

        return File(
            pdf,
            "application/pdf",
            nombreArchivo
        );
    }
}