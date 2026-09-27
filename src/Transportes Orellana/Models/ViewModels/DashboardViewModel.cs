namespace Transportes_Orellana.Models.ViewModels;

public class DashboardViewModel
{
    public int ViajesEnProceso {get; set;}
    public int ViajesProgramados {get; set;}
    public int CamionesDisponibles {get; set;}
    public int ClientesActivos {get; set;}
    public decimal GastosMes {get; set;}
    public decimal GananciaEstimada {get; set;}

    public List<ViajeDashboardItemViewModel> ViajesProximos {get; set;} = new();
    public List<ViajeDashboardItemViewModel> ViajesConQueja {get; set;} = new();
    public List<ClienteRendimientoViewModel> ClientesTop {get; set;} = new();
}