using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Transportes_Orellana.Models.ViewModels;
using Transportes_Orellana.Services.Implementations;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Controllers;

[Authorize(Roles = "Admin")]
public class MotoristasController : Controller
{
    private readonly IMotoristaService _motoristaService;

    public MotoristasController(IMotoristaService motoristaService){
        _motoristaService = motoristaService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new RegistrarMotoristaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult>Create(RegistrarMotoristaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (exito, error) = await _motoristaService.RegistrarMotoristaAsync(model);
        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al procesar el registro");
            return View(model);
        }
        TempData["SuccessMessage"] = $"Motorista {model.Nombre} {model.Apellido} registrado exitosamente";
        return RedirectToAction("Index");
    }

}