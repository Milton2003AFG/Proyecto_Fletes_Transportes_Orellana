namespace Transportes_Orellana.Models.ViewModels;

public class CalculoUtilidadViewModel
{
    public int FleteId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Motorista { get; set; } = string.Empty;

    public string Unidad { get; set; } = string.Empty;

    public string LugarRecolecta { get; set; } = string.Empty;

    public string LugarEntrega { get; set; } = string.Empty;

    public decimal ValorFlete { get; set; }

    public decimal TotalGastos { get; set; }

    public decimal UtilidadNeta { get; set; }

    public decimal PorcentajeRentabilidad { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string Estado { get; set; } = string.Empty;
}