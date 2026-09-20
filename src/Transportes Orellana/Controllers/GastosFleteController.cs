using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transportes_Orellana.Models;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Controllers;

[Authorize(Roles = "Admin,Motorista")]

public class GastosFleteController : Controller
{
    private readonly IGastoFleteService _gastoFleteService;

    public GastosFleteController (IGastoFleteService gastoFleteService)
    {
        _gastoFleteService = gastoFleteService;
    }

    public async Task<IActionResult> Index()
    {
        var resumenGastosPorFlete = await _gastoFleteService.ListarResumenGastosPorFleteAsync();
        return View(resumenGastosPorFlete);
    } 

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(int? id)
    {
        if(id == null) return NotFound();
        var (exito, error, gastos) = await _gastoFleteService.ObtenerDetalleGastosFleteAsync(id.Value);
        if (!exito)
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Index));
        }
        ViewBag.FleteId = id.Value;
        return View(gastos);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create(int fleteId)
    {
        var modelo = new GastoFlete { FleteId = fleteId };
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(GastoFlete gastoFlete)
    {
        ModelState.Remove(nameof(gastoFlete.Flete));
        ModelState.Remove(nameof(gastoFlete.FechaRegistro));

        if(!ModelState.IsValid) return View(gastoFlete);
        var (exito, error) = await _gastoFleteService.CrearGastoAsync(gastoFlete);
        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al registrar gasto.");
            return View(gastoFlete);
        }
        TempData["Exito"] = "Gasto registrado correctamente";
        return RedirectToAction(nameof(Details), new { id = gastoFlete.FleteId});
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if(id == null) return NotFound();
        var gasto = await _gastoFleteService.ObtenerPorIdAsync(id.Value);
        if(gasto == null) return NotFound();
        return View(gasto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, GastoFlete gastoFlete)
    {
        if(id != gastoFlete.Id) return NotFound();
        ModelState.Remove(nameof(gastoFlete.FechaRegistro));
        if(!ModelState.IsValid) return View(gastoFlete);

        var(exito, error) = await _gastoFleteService.ActualizarAsync(gastoFlete);
        if (!exito)
        {
            ModelState.AddModelError(nameof(gastoFlete.Id), error ?? "Error al actualizar gasto.");
            return View(gastoFlete);
        }
        TempData["Exito"] = "Gasto registrado correctamente";
        return RedirectToAction(nameof(Details), new {id = gastoFlete.FleteId});
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null) return NotFound();
        var gasto = await _gastoFleteService.ObtenerPorIdAsync(id.Value);
        if(gasto == null) return NotFound();
        return View(gasto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var gasto = await _gastoFleteService.ObtenerPorIdAsync(id);
        if (gasto == null) return NotFound();

        var fleteId = gasto.FleteId;
        await _gastoFleteService.EliminarAsync(id);
        TempData["Exito"] = "Gasto eliminado correctamente";
        return RedirectToAction(nameof(Details), new {id = fleteId});
    }
}