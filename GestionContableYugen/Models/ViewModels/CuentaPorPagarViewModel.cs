using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.ViewModels
{
    public class CuentaPorPagarViewModel
    {
        public int CuentaID { get; set; }

        [Required]
        public int ProveedorID { get; set; }

        public string NombreProveedor { get; set; }
        public string NumeroCedula { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0.")]
        public decimal Monto { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Required]
        public string Estado { get; set; } = "Pendiente";
    }

}
