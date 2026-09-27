using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Models.ViewModels;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> ObtenerResumenGeneralAsync()
    {
        var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var viajesEnProceso = await _context.Fletes.CountAsync(f => f.Estado == "en_proceso");
        var viajesProgramados = await _context.Fletes.CountAsync(f => f.Estado == "programado");
        var clientesActivos = await _context.Clientes.CountAsync(c => c.Estado == "activo");

        var CamionesOcupadosIds = await _context.Fletes
            .Where(f => f.Estado == "en_proceso" || f.Estado == "programado" || f.Estado == "con_devolucion")
            .Select(f => f.UnidadId)
            .Distinct()
            .ToListAsync();

        var camionesDisponibles = await _context.UnidadesTransporte
            .CountAsync(u => u.Estado == "activo" && !CamionesOcupadosIds.Contains(u.Id));

        var cobrosMes = await _context.Fletes
            .Where(f => f.FechaRegistro >= inicioMes && f.Estado == "terminado")
            .SumAsync(f => (decimal?)f.MontoCobro) ?? 0m;

        var gastosMes = await _context.GastosFletes
            .Where(g => g.FechaRegistro >= inicioMes)
            .SumAsync(g => (decimal?)g.Monto) ?? 0m;

        var proximos = await _context.Fletes
            .Include(f => f.Cliente)
            .Include(f => f.Motorista)
            .Where(f => f.Estado == "programado")
            .OrderBy(f => f.HoraSalida)
            .Take(3)
            .Select(f => new ViajeDashboardItemViewModel
            {
                FleteId = f.Id,
                Cliente = f.Cliente != null ? $"{f.Cliente.Nombre} {f.Cliente.Apellido}" : "N/A",
                Origen = f.LugarRecolecta,
                Destino = f.LugarEntrega,
                FechaSalida = f.HoraSalida,
                Estado = f.Estado,
                Motorista = f.Motorista != null ? $"{f.Motorista.Nombre} {f.Motorista.Apellido}" : "Sin asignar"
            }).ToListAsync();

        var conQueja = await _context.Fletes
            .Include(f => f.Cliente)
            .Include(f => f.Motorista)
            .Where(f => f.Estado == "con_queja")
            .OrderByDescending(f => f.FechaRegistro)
            .Take(3)
            .Select(f => new ViajeDashboardItemViewModel
            {
                FleteId = f.Id,
                Cliente = f.Cliente != null ? $"{f.Cliente.Nombre} {f.Cliente.Apellido}" : "N/A",
                Origen = f.LugarRecolecta,
                Destino = f.LugarEntrega,
                FechaSalida = f.HoraSalida,
                Estado = f.Estado,
                Motorista = f.Motorista != null ? $"{f.Motorista.Nombre} {f.Motorista.Apellido}" : "Sin asignar"
            }).ToListAsync();

        var topClientes = await _context.Fletes
            .Where(f => f.FechaRegistro >= inicioMes && f.Cliente != null)
            .GroupBy(f => f.Cliente!)
            .Select(g => new
            {
                Nombre = g.Key.Nombre,
                Apellido = g.Key.Apellido,
                TotalFletesMes = g.Count()
            })
            .OrderByDescending(c => c.TotalFletesMes)
            .Take(5)
            .ToListAsync();

        var clientesTopModel = topClientes
            .Select(c => new ClienteRendimientoViewModel
            {
                NombreCompleto = $"{c.Nombre} {c.Apellido}".Trim(),
                TotalFletesMes = c.TotalFletesMes
            })
            .ToList();

        return new DashboardViewModel
        {
            ViajesEnProceso = viajesEnProceso,
            ViajesProgramados = viajesProgramados,
            CamionesDisponibles = camionesDisponibles,
            ClientesActivos = clientesActivos,
            GastosMes = gastosMes,
            GananciaEstimada = cobrosMes - gastosMes,
            ViajesProximos = proximos,
            ViajesConQueja = conQueja,
            ClientesTop = clientesTopModel
        };

    }
}