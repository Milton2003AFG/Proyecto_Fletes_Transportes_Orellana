namespace Transportes_Orellana.Services.Interfaces;

public interface IUsuarioService
{
    Task<(bool Exito, string? Error, string? UsuarioId)>RegistrarUsuarioAsync(
        string email,
        string password,
        string rol,
        string? telefono = null);

    Task<(bool Exito, string? Error)>CambiarEstadoUsuarioAsync(string usuarioId, bool bloquear);
}