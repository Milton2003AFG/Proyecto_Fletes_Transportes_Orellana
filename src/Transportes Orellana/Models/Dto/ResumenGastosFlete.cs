namespace Transportes_Orellana.Models.Dto;

public class ResumenGastosFlete
{
    public int FleteId {get; set;}
    public string CodigoViaje => $"Viaje {FleteId:D3}";
    public string ClienteNombre {get; set;} = string.Empty;
    public decimal MontoCobro {get; set;}
    public decimal TotalGasto {get; set;}
    public int CantidadGastos {get; set;}
}