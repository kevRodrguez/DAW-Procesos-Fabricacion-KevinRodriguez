using System.ComponentModel.DataAnnotations;

namespace DAW_Procesos_Fabricacion_KevinRodriguez.Models;

public class ProcesoFabricacion
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del proceso es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    [Display(Name = "Nombre del proceso")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción del proceso es obligatoria.")]
    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    public ICollection<OrdenProceso> Ordenes { get; set; } = new List<OrdenProceso>();
}
