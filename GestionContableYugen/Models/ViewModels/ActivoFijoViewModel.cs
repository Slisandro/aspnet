using System;
using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.ViewModels
{
    public class ActivoFijoViewModel
    {
        public int ActivoID { get; set; }

        [Required(ErrorMessage = "El nombre del activo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string NombreActivo { get; set; }

        [Required(ErrorMessage = "La fecha de adquisición es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaAdquisicion { get; set; }

        [Required(ErrorMessage = "El valor inicial es obligatorio.")]
        [Range(1, double.MaxValue, ErrorMessage = "El valor inicial debe ser mayor a 0.")]
        public decimal ValorInicial { get; set; }

        [Required(ErrorMessage = "Debe especificar la vida útil en años.")]
        [Range(1, 100, ErrorMessage = "La vida útil debe estar entre 1 y 100 años.")]
        public int VidaUtil { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El valor depreciado no puede ser negativo.")]
        public decimal ValorDepreciado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        public string Estado { get; set; }

        public DateTime? UltimaFechaMantenimiento { get; set; } = null;
        public string? ComentarioMantenimiento { get; set; } = "No registrado";
    }
}
