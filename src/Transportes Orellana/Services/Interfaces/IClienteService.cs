using Transportes_Orellana.Models;

namespace Transportes_Orellana.Services.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ObtenerTodosAsync();
    Task<Cliente?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string? Error)> CrearAsync(Cliente cliente);
    Task<(bool Exito, string? Error)> ActualizarAsync(Cliente cliente);
    Task<bool> CambiarEstadoAsync(int id, string nuevoEstado);
    Task<bool> ExisteAsync(int id);
}