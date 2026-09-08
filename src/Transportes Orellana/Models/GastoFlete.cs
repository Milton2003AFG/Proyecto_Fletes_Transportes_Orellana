using Transportes_Orellana.Models;

public class GastoFlete
{
    public int Id { get; set; }
    public int FleteId { get; set; }
    public string TipoGasto { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Cada gasto pertenece a un flete
    public Flete? Flete { get; set; }
}