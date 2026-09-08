using System.ComponentModel.DataAnnotations;

namespace Transportes_Orellana.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Estado { get; set; } = "activo";
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Un cliente puede tener muchos fletes
    public ICollection<Flete>? Fletes { get; set; }
}