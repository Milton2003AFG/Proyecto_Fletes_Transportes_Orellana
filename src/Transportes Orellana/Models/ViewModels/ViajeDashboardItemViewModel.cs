namespace Transportes_Orellana.Models.ViewModels;

public class ViajeDashboardItemViewModel
{
    public int FleteId {get; set;}
    public string CodigoViaje => $"VJ-{FleteId:D3}";
    public string Cliente {get; set;} = string.Empty;
    public string Origen {get; set;} = string.Empty;
    public string Destino {get; set;} = string.Empty;
    public DateTime FechaSalida {get; set;}
    public string Estado {get; set;} = string.Empty;
    public string Motorista {get; set;} = string.Empty;
}