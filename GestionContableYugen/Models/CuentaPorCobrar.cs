using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionContableYugen.Models
{
    public class CuentaPorCobrar
    {
        [Key]
        public int CuentaID { get; set; }

        [Required]
        public int ClienteID { get; set; } // Relacionado con la tabla Clientes

        [Required]
        public string NumeroCedula { get; set; }

        public string Telefono { get; set; } = "No disponible";

        public string CorreoElectronico { get; set; } = "No disponible";

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Required]
        public string Estado { get; set; }

        // Relación con Cliente
        [ForeignKey("ClienteID")]
        public Cliente Cliente { get; set; }
    }

}
