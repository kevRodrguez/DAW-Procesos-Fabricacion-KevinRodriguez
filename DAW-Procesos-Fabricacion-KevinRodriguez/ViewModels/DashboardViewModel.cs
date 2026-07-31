using DAW_Procesos_Fabricacion_KevinRodriguez.Models;

namespace DAW_Procesos_Fabricacion_KevinRodriguez.ViewModels;

public class DashboardViewModel
{
    public int TotalOrdenes { get; set; }

    public int OrdenesPendientes { get; set; }

    public int OrdenesEnProceso { get; set; }

    public int OrdenesCompletadas { get; set; }

    public List<OrdenProduccion> OrdenesRecientes { get; set; } = [];
}
