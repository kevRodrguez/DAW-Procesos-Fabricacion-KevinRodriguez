using System.ComponentModel.DataAnnotations;

namespace DAW_Procesos_Fabricacion_KevinRodriguez.Models;

public class OrdenProceso
{
    public int OrdenProduccionId { get; set; }

    public OrdenProduccion OrdenProduccion { get; set; } = null!;

    public int ProcesoFabricacionId { get; set; }

    public ProcesoFabricacion ProcesoFabricacion { get; set; } = null!;

    [Display(Name = "Estado")]
    public EstadoProceso Estado { get; set; } = EstadoProceso.Pendiente;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de completado")]
    public DateOnly? FechaCompletado { get; set; }
}
