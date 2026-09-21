using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Models;

namespace Transportes_Orellana.Controllers
{
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

            // 1. Validar que la hora de destino sea estrictamente mayor a la hora de salida
            if (viaje.HoraDestino <= viaje.HoraSalida)
            {
                ModelState.AddModelError("HoraDestino", "La hora de destino debe ser posterior a la hora de salida.");
            }

            // 2. Validar que el monto sea válido
            if (viaje.MontoCobro <= 0)
            {
                ModelState.AddModelError("MontoCobro", "El monto de cobro debe ser mayor a 0.");
            }

            // 3. Validación de solapamiento
            var estadosQueBloquean = new[] { "programado", "en_proceso", "con_devolucion" };

            var viajesActivos = await _context.Fletes
                .Where(f => f.Estado != null && estadosQueBloquean.Contains(f.Estado.ToLower()))
                .ToListAsync();

            var solapamiento = viajesActivos.FirstOrDefault(f =>
                (f.MotoristaId == viaje.MotoristaId || f.UnidadId == viaje.UnidadId) &&
                (viaje.HoraSalida < f.HoraDestino && viaje.HoraDestino > f.HoraSalida)
            );

            if (solapamiento != null)
            {
                if (solapamiento.MotoristaId == viaje.MotoristaId)
                {
                    ModelState.AddModelError("MotoristaId", $"El motorista ya tiene asignado el viaje #{solapamiento.Id} en ese rango de tiempo ({solapamiento.HoraSalida:dd/MM/yyyy HH:mm} - {solapamiento.HoraDestino:dd/MM/yyyy HH:mm}).");
                }
                if (solapamiento.UnidadId == viaje.UnidadId)
                {
                    ModelState.AddModelError("UnidadId", $"El camión ya está asignado al viaje #{solapamiento.Id} en ese rango de tiempo ({solapamiento.HoraSalida:dd/MM/yyyy HH:mm} - {solapamiento.HoraDestino:dd/MM/yyyy HH:mm}).");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Fletes.Add(viaje);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "¡Viaje registrado correctamente!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError("", "Ocurrió un error al guardar: " + mensajeReal);
                }
            }

            // Si hay un error, recargamos las listas pasando el objeto para que no se pierda la selección del usuario
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

            if (viaje.HoraDestino <= viaje.HoraSalida)
            {
                ModelState.AddModelError("HoraDestino", "La hora de destino debe ser posterior a la hora de salida.");
            }

            if (viaje.MontoCobro <= 0)
            {
                ModelState.AddModelError("MontoCobro", "El monto de cobro debe ser mayor a 0.");
            }

            // Solapamiento en edición (excluyendo el viaje actual)
            var estadosQueBloquean = new[] { "programado", "en_proceso", "con_devolucion" };

            var viajesActivos = await _context.Fletes
                .Where(f => f.Id != viaje.Id && f.Estado != null && estadosQueBloquean.Contains(f.Estado.ToLower()))
                .ToListAsync();

            var solapamiento = viajesActivos.FirstOrDefault(f =>
                (f.MotoristaId == viaje.MotoristaId || f.UnidadId == viaje.UnidadId) &&
                (viaje.HoraSalida < f.HoraDestino && viaje.HoraDestino > f.HoraSalida)
            );

            if (solapamiento != null)
            {
                if (solapamiento.MotoristaId == viaje.MotoristaId)
                {
                    ModelState.AddModelError("MotoristaId", $"El motorista ya está ocupado en el viaje #{solapamiento.Id}.");
                }
                if (solapamiento.UnidadId == viaje.UnidadId)
                {
                    ModelState.AddModelError("UnidadId", $"El camión ya está ocupado en el viaje #{solapamiento.Id}.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viaje);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Viaje actualizado correctamente.";
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
}