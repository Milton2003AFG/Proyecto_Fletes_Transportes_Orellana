using Transportes_Orellana.Models;

public class UnidadTransporte
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public decimal CapacidadToneladas { get; set; }
    public int AnioFabricacion { get; set; }
    public string Estado { get; set; } = "activo";
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Una unidad puede tener muchos fletes
    public ICollection<Flete>? Fletes { get; set; }
}