using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Models;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Services.Implementations
{
    public class UnidadTransporteService : IUnidadTransporteService
    {
        private readonly AppDbContext _context;

        public UnidadTransporteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UnidadTransporte>> ObtenerTodosAsync(bool soloDisponibles = false)
        {
            var query = _context.UnidadesTransporte.AsQueryable();
            if (soloDisponibles)
            {
                var estadosOcupados = new[] {"en_proceso", "programado", "con_devolucion"};
                query = query.Where(u => u.Estado == "activo" &&
                    !_context.Fletes.Any(f => f.UnidadId == u.Id && estadosOcupados.Contains(f.Estado)));
            }

            return await query.ToListAsync();
        }

        public async Task<UnidadTransporte?> ObtenerPorIdAsync(int id)
        {
            return await _context.UnidadesTransporte
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<(bool exito, string? error)> CrearAsync(
            UnidadTransporte unidad)
        {
            var placaExiste = await _context.UnidadesTransporte
                .AnyAsync(u => u.Placa == unidad.Placa);

            if (placaExiste)
                return (false, "Ya existe una unidad registrada con esta placa.");

            unidad.FechaRegistro = DateTime.UtcNow;
            unidad.Estado = "activo";

            _context.UnidadesTransporte.Add(unidad);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool exito, string? error)> ActualizarAsync(
            UnidadTransporte unidad)
        {
            var existente = await _context.UnidadesTransporte
                .FirstOrDefaultAsync(u => u.Id == unidad.Id);

            if (existente == null)
                return (false, "La unidad de transporte no existe.");

            var placaExiste = await _context.UnidadesTransporte
                .AnyAsync(u =>
                    u.Placa == unidad.Placa &&
                    u.Id != unidad.Id);

            if (placaExiste)
                return (false, "Ya existe otra unidad registrada con esta placa.");

            existente.Placa = unidad.Placa;
            existente.Marca = unidad.Marca;
            existente.CapacidadToneladas = unidad.CapacidadToneladas;
            existente.AnioFabricacion = unidad.AnioFabricacion;

            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<bool> CambiarEstadoAsync(
            int id,
            string estado)
        {
            var unidad = await _context.UnidadesTransporte
                .FirstOrDefaultAsync(u => u.Id == id);

            if (unidad == null)
                return false;

            unidad.Estado = estado;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}