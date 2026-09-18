using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Controllers;

[Authorize(Roles = "Admin,Motorista")]
public class CalculoUtilidadesController : Controller
{
    private readonly ICalculoUtilidadesService _calculoUtilidadesService;

    public CalculoUtilidadesController(
        ICalculoUtilidadesService calculoUtilidadesService)
    {
        _calculoUtilidadesService = calculoUtilidadesService;
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
}