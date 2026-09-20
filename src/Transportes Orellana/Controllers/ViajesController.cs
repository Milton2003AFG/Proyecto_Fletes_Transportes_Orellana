using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Models;

namespace Transportes_Orellana.Controllers;

[Authorize(Roles = "Admin,Motorista")]
public class ViajesController : Controller
{
    private readonly AppDbContext _context;

    public ViajesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Viajes/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var listaViajes = await _context.Fletes
            .Include(v => v.Cliente)
            .Include(v => v.Unidad)
            .Include(v => v.Motorista)
            .ToListAsync();

        return View(listaViajes);
    }

    // GET: /Viajes/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var viaje = await _context.Fletes
            .Include(v => v.Cliente)
            .Include(v => v.Unidad)
            .Include(v => v.Motorista)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (viaje == null) return NotFound();

        return View(viaje);
    }

    // GET: /Viajes/Create
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarListasDesplegablesAsync();
        return View();
    }

    // POST: /Viajes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Flete viaje)
    {
        ModelState.Remove("Cliente");
        ModelState.Remove("Unidad");
        ModelState.Remove("Motorista");
        ModelState.Remove("Gastos");

        // Validación: La fecha y hora de salida no puede ser anterior a la actual
        if (viaje.HoraSalida < DateTime.Now)
        {
            ModelState.AddModelError("HoraSalida", "La fecha y hora de salida no puede ser anterior a la actual.");
        }

        // Validación: El monto de cobro no puede ser menor o igual a 0
        if (viaje.MontoCobro <= 0)
        {
            ModelState.AddModelError("MontoCobro", "El monto de cobro debe ser mayor a 0.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Fletes.Add(viaje);
                await _context.SaveChangesAsync();

                TempData["Exito"] = "¡Viaje registrado correctamente!";

                // Validación: Si se crea como finalizado, redirigir a gastos
                if (!string.IsNullOrEmpty(viaje.Estado) &&
                    (viaje.Estado.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) ||
                     viaje.Estado.Equals("Completado", StringComparison.OrdinalIgnoreCase) ||
                     viaje.Estado.Equals("terminado", StringComparison.OrdinalIgnoreCase)))
                {
                    return RedirectToAction("Index", "Gastos", new { fleteId = viaje.Id });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError("", "Ocurrió un error al guardar el viaje: " + mensajeReal);
            }
        }
        else
        {
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"CAMPO FALLIDO: {key} -> ERROR: {error.ErrorMessage}");
                }
            }
        }

        await CargarListasDesplegablesAsync(viaje);
        return View(viaje);
    }

    // GET: /Viajes/Edit/5
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var viaje = await _context.Fletes.FindAsync(id);
        if (viaje == null) return NotFound();

        await CargarListasDesplegablesAsync(viaje);
        return View(viaje);
    }

    // POST: /Viajes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Flete viaje)
    {
        if (id != viaje.Id) return NotFound();

        ModelState.Remove("Cliente");
        ModelState.Remove("Unidad");
        ModelState.Remove("Motorista");
        ModelState.Remove("Gastos");

        // Validación: La fecha y hora de salida no puede ser anterior a la actual
        if (viaje.HoraSalida < DateTime.Now)
        {
            ModelState.AddModelError("HoraSalida", "La fecha y hora de salida no puede ser anterior a la actual.");
        }

        // Validación: El monto de cobro no puede ser menor o igual a 0
        if (viaje.MontoCobro <= 0)
        {
            ModelState.AddModelError("MontoCobro", "El monto de cobro debe ser mayor a 0.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(viaje);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Viaje actualizado correctamente.";

                // Validación: Si al editar se cambia a finalizado, redirigir a gastos
                if (!string.IsNullOrEmpty(viaje.Estado) &&
                    (viaje.Estado.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) ||
                     viaje.Estado.Equals("Completado", StringComparison.OrdinalIgnoreCase) ||
                     viaje.Estado.Equals("terminado", StringComparison.OrdinalIgnoreCase)))
                {
                    return RedirectToAction("Index", "Gastos", new { fleteId = viaje.Id });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FleteExists(viaje.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        await CargarListasDesplegablesAsync(viaje);
        return View(viaje);
    }

    // GET: /Viajes/Delete/5
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var viaje = await _context.Fletes
            .Include(v => v.Cliente)
            .Include(v => v.Unidad)
            .Include(v => v.Motorista)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (viaje == null) return NotFound();

        return View(viaje);
    }

    // POST: /Viajes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var viaje = await _context.Fletes.FindAsync(id);
        if (viaje != null)
        {
            _context.Fletes.Remove(viaje);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Viaje eliminado correctamente.";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool FleteExists(int id)
    {
        return _context.Fletes.Any(e => e.Id == id);
    }

    private async Task CargarListasDesplegablesAsync(Flete? viaje = null)
    {
        var clientes = await _context.Clientes.ToListAsync();
        var unidades = await _context.UnidadesTransporte.ToListAsync();
        var motoristas = await _context.Motoristas.ToListAsync();

        ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nombre", viaje?.ClienteId);
        ViewData["UnidadId"] = new SelectList(unidades, "Id", "Placa", viaje?.UnidadId);
        ViewData["MotoristaId"] = new SelectList(motoristas, "Id", "Nombre", viaje?.MotoristaId);
    }
}