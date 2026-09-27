using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transportes_Orellana.Models;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Controllers;

// Admin y Motorista pueden consultar las unidades
[Authorize(Roles = "Admin,Motorista")]
public class UnidadesTransporteController : Controller
{
    private readonly IUnidadTransporteService _unidadTransporteService;

    public UnidadesTransporteController(
        IUnidadTransporteService unidadTransporteService)
    {
        _unidadTransporteService = unidadTransporteService;
    }

    // GET: UnidadesTransporte
    // Admin y Motorista
    public async Task<IActionResult> Index()
    {
        var unidades =
            await _unidadTransporteService.ObtenerTodosAsync();

        return View(unidades);
    }

    // GET: UnidadesTransporte/Details/5
    // Admin y Motorista
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var unidad =
            await _unidadTransporteService.ObtenerPorIdAsync(id.Value);

        if (unidad == null)
            return NotFound();

        return View(unidad);
    }

    // GET: UnidadesTransporte/Create
    // Solo Admin
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: UnidadesTransporte/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(UnidadTransporte unidad)
    {
        // Estos valores los controla el sistema
        ModelState.Remove(nameof(unidad.FechaRegistro));
        ModelState.Remove(nameof(unidad.Estado));

        if (!ModelState.IsValid)
            return View(unidad);

        var (exito, error) =
            await _unidadTransporteService.CrearAsync(unidad);

        if (!exito)
        {
            ModelState.AddModelError(
                nameof(unidad.Placa),
                error ?? "Error al registrar la unidad.");

            return View(unidad);
        }

        TempData["Exito"] =
            "Unidad de transporte registrada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // GET: UnidadesTransporte/Edit/5
    // Solo Admin
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var unidad =
            await _unidadTransporteService.ObtenerPorIdAsync(id.Value);

        if (unidad == null)
            return NotFound();

        return View(unidad);
    }

    // POST: UnidadesTransporte/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(
        int id,
        UnidadTransporte unidad)
    {
        if (id != unidad.Id)
            return NotFound();

        ModelState.Remove(nameof(unidad.FechaRegistro));
        ModelState.Remove(nameof(unidad.Estado));

        if (!ModelState.IsValid)
            return View(unidad);

        var (exito, error) =
            await _unidadTransporteService.ActualizarAsync(unidad);

        if (!exito)
        {
            ModelState.AddModelError(
                string.Empty,
                error ?? "Error al actualizar la unidad.");

            return View(unidad);
        }

        TempData["Exito"] =
            "Unidad de transporte actualizada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // GET: UnidadesTransporte/Activate/5
    // Solo Admin
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(int? id)
    {
        if (id == null)
            return NotFound();

        var unidad =
            await _unidadTransporteService.ObtenerPorIdAsync(id.Value);

        if (unidad == null)
            return NotFound();

        return View(unidad);
    }

    // POST: UnidadesTransporte/ActivateConfirmed
    [HttpPost, ActionName("ActivateConfirmed")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateConfirmed(int id)
    {
        await _unidadTransporteService
            .CambiarEstadoAsync(id, "activo");

        TempData["Exito"] =
            "Unidad de transporte activada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // GET: UnidadesTransporte/Delete/5
    // En realidad DESACTIVA, no elimina físicamente
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var unidad =
            await _unidadTransporteService.ObtenerPorIdAsync(id.Value);

        if (unidad == null)
            return NotFound();

        return View(unidad);
    }

    // POST: UnidadesTransporte/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _unidadTransporteService
            .CambiarEstadoAsync(id, "inactivo");

        TempData["Exito"] =
            "Unidad de transporte desactivada correctamente.";

        return RedirectToAction(nameof(Index));
    }
}