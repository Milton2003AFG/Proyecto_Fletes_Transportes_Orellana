using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transportes_Orellana.Models;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Controllers;

// Requiere que el usuario esté logueado (Admin o Motorista) para entrar al controlador
[Authorize(Roles = "Admin,Motorista")]
public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    // Permitido para Admin y Motorista
    public async Task<IActionResult> Index()
    {
        var clientes = await _clienteService.ObtenerTodosAsync();
        return View(clientes);
    }

    // Permitido para Admin y Motorista
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var cliente = await _clienteService.ObtenerPorIdAsync(id.Value);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    // Acciones exclusivas para Admin

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        if (!ModelState.IsValid) return View(cliente);
        var (exito, error) = await _clienteService.CrearAsync(cliente);
        if (!exito)
        {
            ModelState.AddModelError(nameof(cliente.Correo), error ?? "Error al registrar cliente.");
            return View(cliente);
        }
        TempData["Exito"] = "Cliente registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var cliente = await _clienteService.ObtenerPorIdAsync(id.Value);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Cliente cliente)
    {
        if (id != cliente.Id) return NotFound();
        ModelState.Remove(nameof(cliente.FechaRegistro));
        if (!ModelState.IsValid) return View(cliente);

        var (exito, error) = await _clienteService.ActualizarAsync(cliente);
        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al actualizar cliente.");
            return View(cliente);
        }

        TempData["Exito"] = "Cliente actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(int? id)
    {
        if (id == null) return NotFound();
        var cliente = await _clienteService.ObtenerPorIdAsync(id.Value);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    [HttpPost, ActionName("ActivateConfirmed")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateConfirmed(int id)
    {
        await _clienteService.CambiarEstadoAsync(id, "activo");
        TempData["Exito"] = "Cliente activado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var cliente = await _clienteService.ObtenerPorIdAsync(id.Value);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _clienteService.CambiarEstadoAsync(id, "inactivo");
        TempData["Exito"] = "Cliente desactivado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}