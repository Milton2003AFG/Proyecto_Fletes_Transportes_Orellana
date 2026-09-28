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
        public async Task<IActionResult> Index(string? estado)
        {
            var lista = _context.Fletes
                .Include(v => v.Cliente)
                .Include(v => v.Unidad)
                .Include(v => v.Motorista)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
            {
                lista = lista.Where(v => v.Estado == estado);
            }

            var listaViajes = await lista.ToListAsync();
            ViewBag.EstadoActual = estado;
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

            // 1. Validar fechas lógicas
            if (viaje.HoraSalida < DateTime.Now.AddMinutes(-30))
            {
                ModelState.AddModelError("HoraSalida", "La fecha de salida no puede ser anterior a la actual.");
            }

            if (viaje.HoraDestino <= viaje.HoraSalida)
            {
                ModelState.AddModelError("HoraDestino", "La hora de destino debe ser posterior a la hora de salida.");
            }

            if (viaje.MontoCobro <= 0)
            {
                ModelState.AddModelError("MontoCobro", "El monto de cobro debe ser mayor a 0.");
            }

            // 2. Validar que Cliente, Camión y Motorista existan y estén activos
            var cliente = await _context.Clientes.FindAsync(viaje.ClienteId);
            if (cliente == null || string.IsNullOrWhiteSpace(cliente.Estado) || !cliente.Estado.Trim().Equals("activo", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("ClienteId", "El cliente seleccionado no existe o no está activo.");
            }

            var unidad = await _context.UnidadesTransporte.FindAsync(viaje.UnidadId);
            if (unidad == null || string.IsNullOrWhiteSpace(unidad.Estado) || !unidad.Estado.Trim().Equals("activo", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("UnidadId", "La unidad de transporte no existe o no está activa.");
            }

            var motorista = await _context.Motoristas.FindAsync(viaje.MotoristaId);
            if (motorista == null || string.IsNullOrWhiteSpace(motorista.Estado) || !motorista.Estado.Trim().Equals("activo", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("MotoristaId", "El motorista no existe o no está activo.");
            }

            // 3. Validación de solapamiento
            var estadosQueBloquean = new[] { "programado", "en_proceso", "con_devolucion" };

            var solapamiento = await _context.Fletes
                .Where(f => f.Estado != null && estadosQueBloquean.Contains(f.Estado.ToLower()))
                .Where(f => (f.MotoristaId == viaje.MotoristaId || f.UnidadId == viaje.UnidadId) &&
                            viaje.HoraSalida < f.HoraDestino && viaje.HoraDestino > f.HoraSalida)
                .FirstOrDefaultAsync();

            if (solapamiento != null)
            {
                if (solapamiento.MotoristaId == viaje.MotoristaId)
                {
                    ModelState.AddModelError("MotoristaId", $"El motorista ya tiene asignado el viaje #{solapamiento.Id} en ese rango ({solapamiento.HoraSalida:dd/MM/yyyy HH:mm} - {solapamiento.HoraDestino:dd/MM/yyyy HH:mm}).");
                }
                if (solapamiento.UnidadId == viaje.UnidadId)
                {
                    ModelState.AddModelError("UnidadId", $"El camión ya está asignado al viaje #{solapamiento.Id} en ese rango ({solapamiento.HoraSalida:dd/MM/yyyy HH:mm} - {solapamiento.HoraDestino:dd/MM/yyyy HH:mm}).");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Fletes.Add(viaje);
                    await _context.SaveChangesAsync();

                    bool esFinalizado = !string.IsNullOrEmpty(viaje.Estado) &&
                        (viaje.Estado.Equals("terminado", StringComparison.OrdinalIgnoreCase) ||
                        viaje.Estado.Equals("con_queja", StringComparison.OrdinalIgnoreCase));

                    if (esFinalizado)
                    {
                        return RedirectToAction("Create", "GastosFlete", new { fleteId = viaje.Id });
                    }

                    TempData["Exito"] = "¡Viaje registrado correctamente!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError("", "Ocurrió un error al guardar: " + mensajeReal);
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

            if (viaje.HoraDestino <= viaje.HoraSalida)
            {
                ModelState.AddModelError("HoraDestino", "La hora de destino debe ser posterior a la hora de salida.");
            }

            if (viaje.MontoCobro <= 0)
            {
                ModelState.AddModelError("MontoCobro", "El monto de cobro debe ser mayor a 0.");
            }

            // 1. Validar finalización anticipada
            bool esFinalizado = !string.IsNullOrEmpty(viaje.Estado) &&
                (viaje.Estado.Equals("terminado", StringComparison.OrdinalIgnoreCase) ||
                viaje.Estado.Equals("con_queja", StringComparison.OrdinalIgnoreCase));

            if (esFinalizado && viaje.HoraDestino > DateTime.Now)
            {
                ModelState.AddModelError("HoraDestino", "Un viaje terminado o con queja no puede tener una hora de llegada en el futuro.");
            }

            // 2. Solapamiento en edición
            var estadosQueBloquean = new[] { "programado", "en_proceso", "con_devolucion" };

            var solapamiento = await _context.Fletes
                .Where(f => f.Id != viaje.Id && f.Estado != null && estadosQueBloquean.Contains(f.Estado.ToLower()))
                .Where(f => (f.MotoristaId == viaje.MotoristaId || f.UnidadId == viaje.UnidadId) &&
                            viaje.HoraSalida < f.HoraDestino && viaje.HoraDestino > f.HoraSalida)
                .FirstOrDefaultAsync();

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

                    if (esFinalizado)
                    {
                        return RedirectToAction("Create", "GastosFlete", new { fleteId = viaje.Id });
                    }

                    TempData["Exito"] = "Viaje actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FleteExists(viaje.Id)) return NotFound();
                    else throw;
                }
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

        // Solo cargar registros activos en los selects
        private async Task CargarListasDesplegablesAsync(Flete? viaje = null)
        {
            // Clientes activos (o el cliente actual si estamos editando)
            var clientes = await _context.Clientes
                .Where(c => (c.Estado != null && c.Estado.ToLower() == "activo") || 
                            (viaje != null && c.Id == viaje.ClienteId))
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            // Unidades activas (o la unidad actual si estamos editando)
            var unidades = await _context.UnidadesTransporte
                .Where(u => (u.Estado != null && u.Estado.ToLower() == "activo") || 
                            (viaje != null && u.Id == viaje.UnidadId))
                .OrderBy(u => u.Placa)
                .ToListAsync();

            // Motoristas activos (o el motorista actual si estamos editando)
            var motoristas = await _context.Motoristas
                .Where(m => (m.Estado != null && m.Estado.ToLower() == "activo") || 
                            (viaje != null && m.Id == viaje.MotoristaId))
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nombre", viaje?.ClienteId);
            ViewData["UnidadId"] = new SelectList(unidades, "Id", "Placa", viaje?.UnidadId);
            ViewData["MotoristaId"] = new SelectList(motoristas, "Id", "Nombre", viaje?.MotoristaId);
        }
    }
}