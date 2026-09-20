using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Models;
using Transportes_Orellana.Models.Dto;
using Transportes_Orellana.Services.Interfaces;
namespace Transportes_Orellana.Services.Implementations;

public class GastoFleteService : IGastoFleteService
{
    private readonly AppDbContext _context;

    public GastoFleteService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<ResumenGastosFlete>> ListarResumenGastosPorFleteAsync()
    {
        return await _context.Fletes
            .AsNoTracking()
            .Where(f => f.Estado == "terminado")
            .Select(f => new ResumenGastosFlete
            {
                FleteId = f.Id,
                ClienteNombre = f.Cliente == null
                    ? string.Empty
                    : f.Cliente.Nombre + " " + f.Cliente.Apellido,
                MontoCobro = f.MontoCobro,
                TotalGasto = f.Gastos == null
                    ? 0m
                    : f.Gastos.Sum(g => (decimal?)g.Monto) ?? 0m,
                CantidadGastos = f.Gastos == null ? 0 : f.Gastos.Count()
            })
            .ToListAsync();
    }

    public async Task<(bool Exito, string? Error)> CrearGastoAsync(GastoFlete gasto)
    {
        if(gasto.Monto <= 0)
        {
            return (false, "El monto debe ser mayor a 0.");
        }

        if(string.IsNullOrWhiteSpace(gasto.Concepto))
        {
            return (false, "El concepto del gasto es obligatorio.");
        }

        if(gasto.Concepto.Length > 100)
        {
            return (false, "El concepto no debe superar los 100 caracteres.");
        }

        var tiposPermitidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "camion", "varios", "produccion"
        };

        if(string.IsNullOrWhiteSpace(gasto.TipoGasto) || !tiposPermitidos.Contains(gasto.TipoGasto.Trim()))
        {
            return (false, "Tipo de gasto no válido. Opciones: camion, varios, produccion.");
        }

        var flete = await _context.Fletes
            .AsNoTracking()
            .Where(f => f.Id == gasto.FleteId)
            .Select(f => new {f.Id, f.Estado})
            .FirstOrDefaultAsync();

        if(flete == null)
        {
            return(false, "El flete no existe");
        }

        if(!string.Equals(flete.Estado, "terminado", StringComparison.OrdinalIgnoreCase))
        {
            return(false, "Solo se pueden registrar gastos en fletes que estén terminados.");
        }

        gasto.TipoGasto = gasto.TipoGasto.Trim().ToLowerInvariant();
        gasto.Concepto = gasto.Concepto.Trim();
        gasto.FechaRegistro = DateTime.UtcNow;
        _context.GastosFletes.Add(gasto);
        await _context.SaveChangesAsync();
        
        return (true, null);
    }

    public async Task<(bool Exito, string? Error, List<GastoItem>? GastoItem)> ObtenerDetalleGastosFleteAsync(int fleteId)
    {
        var flete = await _context.Fletes
            .AsNoTracking()
            .Where(f => f.Id == fleteId)
            .Select(f => new
            { f.Id, f.Estado})
            .FirstOrDefaultAsync();

        if(flete == null)
        {
            return (false, "El flete no existe", null);
        }

        if (!string.Equals(flete.Estado, "terminado", StringComparison.OrdinalIgnoreCase))
        {
            return (false, "Solo se pueden gestionar gastos de fletes terminados.", null);
        }

        var gasto = await _context.GastosFletes
            .AsNoTracking()
            .Where(g => g.FleteId == fleteId)
            .Select(g => new GastoItem
            {
                GastoId = g.Id,
                Concepto = g.Concepto,
                TipoGasto = g.TipoGasto,
                Monto = g.Monto,
                FechaRegistro = g.FechaRegistro

            })
            .ToListAsync();

        return(true, null, gasto);
    }   

    public async Task<(bool Exito, string? Error)> ActualizarAsync(GastoFlete gasto)
    {
        var gastoDb = await _context.GastosFletes.FindAsync(gasto.Id);
        if(gastoDb == null)
        {
            return (false, "El gasto no existe.");
        }

        if(gasto.Monto <= 0)
        {
            return (false, "El monto debe ser mayor a 0.");
        }

        if(string.IsNullOrWhiteSpace(gasto.Concepto))
        {
            return (false, "El concepto del gasto es obligatorio.");
        }

        if(gasto.Concepto.Length > 100)
        {
            return (false, "El concepto no debe superar los 100 caracteres.");
        }

        var tiposPermitidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "camion", "varios", "produccion"
        };

        if(string.IsNullOrWhiteSpace(gasto.TipoGasto) || !tiposPermitidos.Contains(gasto.TipoGasto.Trim()))
        {
            return (false, "Tipo de gasto no válido. Opciones: camion, varios, produccion.");
        }

        gastoDb.TipoGasto = gasto.TipoGasto.Trim().ToLowerInvariant();
        gastoDb.Concepto = gasto.Concepto.Trim();
        gastoDb.Monto = gasto.Monto;
        await _context.SaveChangesAsync();
        return (true, null);
    } 

    public async Task<GastoFlete?> ObtenerPorIdAsync(int id)
    {
        return await _context.GastosFletes.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<(bool Exito, string? Error)> EliminarAsync(int id)
    {
        try
        {
            var gastoInfo = await _context.GastosFletes
                .Where(g => g.Id == id)
                .Select(g => new
                {
                    g.Id,
                    EstadoFlete = g.Flete != null ? g.Flete.Estado : default
                })
                .FirstOrDefaultAsync();

            if(gastoInfo == null)
            {
                return (false, "El gasto específico no existe");
            }

            if (!string.Equals(gastoInfo.EstadoFlete, "terminado", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Solo se pueden eliminar gastos de fletes que estén terminados.");
            }

            await _context.GastosFletes
                .Where(g => g.Id == id)
                .ExecuteDeleteAsync();

            return(true, null);
        }
        catch (DbException ex)
        {
            return (false, $"Error de base de datos al eliminar el gasto: {ex.Message}");
        }
    }
}