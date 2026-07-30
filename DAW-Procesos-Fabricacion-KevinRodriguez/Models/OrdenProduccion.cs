using System.ComponentModel.DataAnnotations;

namespace DAW_Procesos_Fabricacion_KevinRodriguez.Models;

public class OrdenProduccion : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El número de orden es obligatorio.")]
    [StringLength(20, ErrorMessage = "El número de orden no puede superar los 20 caracteres.")]
    [Display(Name = "Número de orden")]
    public string NumeroOrden { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo de calzado es obligatorio.")]
    [StringLength(100, ErrorMessage = "El modelo de calzado no puede superar los 100 caracteres.")]
    [Display(Name = "Modelo de calzado")]
    public string ModeloCalzado { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad a producir debe ser mayor que cero.")]
    [Display(Name = "Cantidad a producir")]
    public int Cantidad { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de creación")]
    public DateOnly FechaCreacion { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha estimada de entrega")]
    public DateOnly FechaEntregaEstimada { get; set; }

    public ICollection<OrdenProceso> Procesos { get; set; } = new List<OrdenProceso>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaCreacion == DateOnly.MinValue)
        {
            yield return new ValidationResult(
                "La fecha de creación es obligatoria y debe ser válida.",
                [nameof(FechaCreacion)]);
        }

        if (FechaEntregaEstimada == DateOnly.MinValue)
        {
            yield return new ValidationResult(
                "La fecha estimada de entrega es obligatoria y debe ser válida.",
                [nameof(FechaEntregaEstimada)]);
        }

        if (FechaCreacion != DateOnly.MinValue &&
            FechaEntregaEstimada != DateOnly.MinValue &&
            FechaEntregaEstimada < FechaCreacion)
        {
            yield return new ValidationResult(
                "La fecha estimada de entrega no puede ser anterior a la fecha de creación.",
                [nameof(FechaEntregaEstimada)]);
        }
    }
}
