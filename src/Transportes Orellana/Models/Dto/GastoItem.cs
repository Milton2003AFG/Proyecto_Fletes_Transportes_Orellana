namespace Transportes_Orellana.Models.Dto;

public class GastoItem
{
    public int GastoId { get; set; }
    public string TipoGasto { get; set; } = string.Empty; 
    public string Concepto { get; set; } = string.Empty;  
    public DateTime FechaRegistro { get; set; }
    public decimal Monto { get; set; }
}