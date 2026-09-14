using Microsoft.AspNetCore.Identity;

namespace Transportes_Orellana.Models;

public class Motorista
{
    public int Id { get; set; }

    // Llave foránea hacia AspNetUsers
    public string? UsuarioId { get; set; }

    // Propiedad de navegación opcional hacia el usuario de Identity
    public IdentityUser? Usuario { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? PerfilSocial { get; set; }
    public string? Curriculum { get; set; }
    public string Estado { get; set; } = "activo";
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Un motorista puede tener muchos fletes
    public ICollection<Flete>? Fletes { get; set; }
}