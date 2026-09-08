using Transportes_Orellana.Models;

public class Flete
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int UnidadId { get; set; }
    public int MotoristaId { get; set; }
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = "programado";
    public decimal MontoCobro { get; set; }
    public string LugarRecolecta { get; set; } = string.Empty;
    public string LugarEntrega { get; set; } = string.Empty;
    public DateTime HoraSalida { get; set; }
    public DateTime HoraDestino { get; set; }
    public string? Consideraciones { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Propiedades de navegación, cada flete pertenece a uno de cada uno
    public Cliente? Cliente { get; set; }
    public UnidadTransporte? Unidad { get; set; }
    public Motorista? Motorista { get; set; }

    // Un flete puede tener muchos gastos
    public ICollection<GastoFlete>? Gastos { get; set; }
}