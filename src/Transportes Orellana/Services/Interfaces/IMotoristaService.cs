using Transportes_Orellana.Models;
using Transportes_Orellana.Models.ViewModels;

namespace Transportes_Orellana.Services.Interfaces;

public interface IMotoristaService
{
    Task<IEnumerable<Motorista>> ObtenerTodosAsync();
    Task<Motorista?> ObtenerPorIdAsync(int id);

    Task<(bool Exito, string? Error)> RegistrarMotoristaAsync(
        RegistrarMotoristaViewModel model);

    Task<(bool Exito, string? Error)> ActualizarAsync(Motorista motorista);

    Task<bool> CambiarEstadoAsync(int id, string nuevoEstado);
}