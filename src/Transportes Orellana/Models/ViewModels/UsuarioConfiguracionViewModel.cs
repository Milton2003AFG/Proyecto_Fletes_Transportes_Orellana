namespace Transportes_Orellana.Models.ViewModels;

public class UsuarioConfiguracionViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Rol { get; set; } = string.Empty;
    public bool EmailConfirmado { get; set; }
}