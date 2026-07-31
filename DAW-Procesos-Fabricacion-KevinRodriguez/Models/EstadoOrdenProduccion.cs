using System.ComponentModel.DataAnnotations;

namespace DAW_Procesos_Fabricacion_KevinRodriguez.Models;

public enum EstadoOrdenProduccion
{
    [Display(Name = "Pendiente")]
    Pendiente,

    [Display(Name = "En proceso")]
    EnProceso,

    [Display(Name = "Completada")]
    Completada
}
