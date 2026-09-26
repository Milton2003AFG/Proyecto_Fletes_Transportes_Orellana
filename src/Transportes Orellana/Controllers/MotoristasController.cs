using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transportes_Orellana.Models;
using Transportes_Orellana.Models.ViewModels;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Controllers;

[Authorize(Roles = "Admin")]
public class MotoristasController : Controller
{
    private readonly IMotoristaService _motoristaService;

    public MotoristasController(IMotoristaService motoristaService)
    {
        _motoristaService = motoristaService;
    }

    public async Task<IActionResult> Index()
        => View(await _motoristaService.ObtenerTodosAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var motorista = await _motoristaService.ObtenerPorIdAsync(id.Value);
        return motorista == null ? NotFound() : View(motorista);
    }

    [HttpGet]
    public IActionResult Create() => View(new RegistrarMotoristaViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegistrarMotoristaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (exito, error) = await _motoristaService.RegistrarMotoristaAsync(model);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al registrar el motorista.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Motorista registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var motorista = await _motoristaService.ObtenerPorIdAsync(id.Value);
        return motorista == null ? NotFound() : View(motorista);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Motorista motorista)
    {
        if (id != motorista.Id) return NotFound();

        ModelState.Remove(nameof(motorista.FechaRegistro));

        if (!ModelState.IsValid) return View(motorista);

        var (exito, error) = await _motoristaService.ActualizarAsync(motorista);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al actualizar el motorista.");
            return View(motorista);
        }

        TempData["SuccessMessage"] = "Motorista actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var motorista = await _motoristaService.ObtenerPorIdAsync(id.Value);
        return motorista == null ? NotFound() : View(motorista);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await _motoristaService.CambiarEstadoAsync(id, "inactivo"))
            return NotFound();

        TempData["SuccessMessage"] = "Motorista eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Activate(int? id)
    {
        if (id == null) return NotFound();

        var motorista = await _motoristaService.ObtenerPorIdAsync(id.Value);
        return motorista == null ? NotFound() : View(motorista);
    }

    [HttpPost, ActionName("Activate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateConfirmed(int id)
    {
        if (!await _motoristaService.CambiarEstadoAsync(id, "activo"))
            return NotFound();

        TempData["SuccessMessage"] = "Motorista agregado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}