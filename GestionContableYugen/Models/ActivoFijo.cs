using System;
using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class ActivoFijo
    {
        [Key]
        public int ActivoID { get; set; }

        [Required]
        public string NombreActivo { get; set; }

        [Required]
        public DateTime FechaAdquisicion { get; set; }

        [Required]
        public decimal ValorInicial { get; set; }

        [Required]
        public int VidaUtil { get; set; }

        [Required]
        public decimal ValorDepreciado { get; set; }

        [Required]
        public string Estado { get; set; }
        public DateTime? UltimaFechaMantenimiento { get; set; } = null;
        public string? ComentarioMantenimiento { get; set; } = null;
    }
}
