using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Models.ViewModels;

namespace Transportes_Orellana.Controllers;

public class CalculoUtilidadesController : Controller
{
    private readonly AppDbContext _context;

    public CalculoUtilidadesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: CalculoUtilidades
    public async Task<IActionResult> Index()
    {
        var fletesTerminados = await _context.Fletes
            .Include(f => f.Cliente)
            .Include(f => f.Motorista)
            .Include(f => f.Unidad)
            .Include(f => f.Gastos)
            .Where(f => f.Estado == "terminado")
            .ToListAsync();

        var utilidades = fletesTerminados.Select(f =>
        {
            var totalGastos = f.Gastos?.Sum(g => g.Monto) ?? 0;

            var utilidadNeta = f.MontoCobro - totalGastos;

            var porcentajeRentabilidad = f.MontoCobro > 0
                ? (utilidadNeta / f.MontoCobro) * 100
                : 0;

            return new CalculoUtilidadViewModel
            {
                FleteId = f.Id,
                Cliente = f.Cliente != null
                    ? $"{f.Cliente.Nombre} {f.Cliente.Apellido}"
                    : "Sin cliente",

                Motorista = f.Motorista != null
                    ? $"{f.Motorista.Nombre} {f.Motorista.Apellido}"
                    : "Sin motorista",

                Unidad = f.Unidad?.Placa ?? "Sin unidad",

                LugarRecolecta = f.LugarRecolecta,
                LugarEntrega = f.LugarEntrega,

                ValorFlete = f.MontoCobro,
                TotalGastos = totalGastos,
                UtilidadNeta = utilidadNeta,
                PorcentajeRentabilidad = porcentajeRentabilidad,

                FechaRegistro = f.FechaRegistro,
                Estado = f.Estado
            };
        }).ToList();

        return View(utilidades);
    }

    // GET: Details
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var flete = await _context.Fletes
            .Include(f => f.Cliente)
            .Include(f => f.Motorista)
            .Include(f => f.Unidad)
            .Include(f => f.Gastos)
            .FirstOrDefaultAsync(f => f.Id == id && f.Estado == "terminado");

        if (flete == null)
        {
            return NotFound();
        }

        var totalGastos = flete.Gastos?.Sum(g => g.Monto) ?? 0;

        var utilidadNeta = flete.MontoCobro - totalGastos;

        var porcentajeRentabilidad = flete.MontoCobro > 0
            ? (utilidadNeta / flete.MontoCobro) * 100
            : 0;

        var viewModel = new CalculoUtilidadViewModel
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
            TotalGastos = totalGastos,
            UtilidadNeta = utilidadNeta,
            PorcentajeRentabilidad = porcentajeRentabilidad,

            FechaRegistro = flete.FechaRegistro,
            Estado = flete.Estado
        };

        return View(viewModel);
    }
}