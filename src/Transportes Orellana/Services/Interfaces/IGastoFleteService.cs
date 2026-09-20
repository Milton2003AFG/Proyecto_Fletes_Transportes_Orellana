using Transportes_Orellana.Models;
using Transportes_Orellana.Models.Dto;

namespace Transportes_Orellana.Services.Interfaces;

public interface IGastoFleteService
{
    Task<(bool Exito, string? Error)> CrearGastoAsync(GastoFlete gasto);
    Task<(bool Exito, string? Error, List<GastoItem>? GastoItem)> ObtenerDetalleGastosFleteAsync(int fleteId);
    Task<(bool Exito, string? Error)> ActualizarAsync(GastoFlete gasto);
    Task<GastoFlete?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string? Error)> EliminarAsync(int id);
    Task<IEnumerable<ResumenGastosFlete>> ListarResumenGastosPorFleteAsync();

}