using Transportes_Orellana.Models.ViewModels;

namespace Transportes_Orellana.Services.Interfaces;

public interface IMotoristaService
{
    Task<(bool Exito, string? Error)> RegistrarMotoristaAsync(RegistrarMotoristaViewModel model);
}