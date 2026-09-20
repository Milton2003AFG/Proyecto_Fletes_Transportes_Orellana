using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Models;
using Transportes_Orellana.Models.ViewModels;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Services.Implementations;

public class CalculoUtilidadesService : ICalculoUtilidadesService
{
    private readonly AppDbContext _context;

    public CalculoUtilidadesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CalculoUtilidadViewModel>> ObtenerTodosAsync()
    {
        var fletes = await _context.Fletes
            .AsNoTracking()
            .Include(f => f.Cliente)
            .Include(f => f.Motorista)
            .Include(f => f.Unidad)
            .Include(f => f.Gastos)
            .Where(f => f.Estado == "terminado")
            .OrderByDescending(f => f.FechaRegistro)
            .ToListAsync();

        return fletes
            .Select(CrearViewModel)
            .ToList();
    }

    public async Task<CalculoUtilidadViewModel?> ObtenerPorFleteAsync(int fleteId)
    {
        var flete = await _context.Fletes
            .AsNoTracking()
            .Include(f => f.Cliente)
            .Include(f => f.Motorista)
            .Include(f => f.Unidad)
            .Include(f => f.Gastos)
            .FirstOrDefaultAsync(f =>
                f.Id == fleteId &&
                f.Estado == "terminado");

        if (flete == null)
        {
            return null;
        }

        return CrearViewModel(flete);
    }

    private CalculoUtilidadViewModel CrearViewModel(Flete flete)
    {
        var gastos = flete.Gastos ?? new List<GastoFlete>();

        var detalleGastos = gastos
            .Select(g => new GastoDetalleViewModel
            {
                TipoGasto = g.TipoGasto,
                Concepto = g.Concepto,
                Monto = g.Monto,
                Grupo = ObtenerNombreGrupo(g.TipoGasto)
            })
            .ToList();

        var gastosCamion = gastos
            .Where(g => string.Equals(
                g.TipoGasto,
                "camion",
                StringComparison.OrdinalIgnoreCase))
            .Sum(g => g.Monto);

        var gastosVarios = gastos
            .Where(g => string.Equals(
                g.TipoGasto,
                "varios",
                StringComparison.OrdinalIgnoreCase))
            .Sum(g => g.Monto);

        var gastosProduccion = gastos
            .Where(g => string.Equals(
                g.TipoGasto,
                "produccion",
                StringComparison.OrdinalIgnoreCase))
            .Sum(g => g.Monto);

        var totalGastos = gastos.Sum(g => g.Monto);

        var utilidadNeta = flete.MontoCobro - totalGastos;

        var porcentajeRentabilidad = flete.MontoCobro > 0
            ? (utilidadNeta / flete.MontoCobro) * 100
            : 0;

        return new CalculoUtilidadViewModel
        {
            FleteId = flete.Id,

            Cliente = flete.Cliente != null
                ? $"{flete.Cliente.Nombre} {flete.Cliente.Apellido}"
                : "Sin cliente",

            Motorista = flete.Motorista != null
                ? $"{flete.Motorista.Nombre} {flete.Motorista.Apellido}"
                : "Sin motorista",

            Unidad = flete.Unidad?.Placa ?? "Sin unidad",

            LugarRecolecta = flete.LugarRecolecta,
            LugarEntrega = flete.LugarEntrega,

            ValorFlete = flete.MontoCobro,

            GastosCamion = gastosCamion,
            GastosVarios = gastosVarios,
            GastosProduccion = gastosProduccion,

            TotalGastos = totalGastos,
            UtilidadNeta = utilidadNeta,
            PorcentajeRentabilidad = porcentajeRentabilidad,

            FechaRegistro = flete.FechaRegistro,
            Estado = flete.Estado,

            Gastos = detalleGastos
        };
    }

    private string ObtenerNombreGrupo(string tipoGasto)
    {
        return tipoGasto.Trim().ToLowerInvariant() switch
        {
            "camion" => "Camión",
            "varios" => "Varios",
            "produccion" => "Producción",
            _ => "Sin clasificar"
        };
    }
}