using GestionContableYugen.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class CuentaPorPagar
    {
        [Key]
        public int CuentaID { get; set; }

        [Required]
        public int ProveedorID { get; set; }

        [ForeignKey("ProveedorID")]
        public Proveedor Proveedor { get; set; } // Relación con la tabla Proveedores

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required]
        public DateTime FechaVencimiento { get; set; }

        [Required]
        public string Estado { get; set; } = "Pendiente";

        // Campos adicionales que no están en la base de datos pero se usan en la vista
        [NotMapped]
        public string NumeroCedula { get; set; } // Número de Cédula del proveedor

        [NotMapped]
        public string Telefono { get; set; } // Teléfono del proveedor

        [NotMapped]
        public string CorreoElectronico { get; set; } // Correo Electrónico del proveedor

        [NotMapped]
        public string NombreProveedor { get; set; } // Nombre del proveedor
    }
}

