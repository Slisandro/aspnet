using System;
using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.ViewModels
{
    public class CuentaPorCobrarViewModel
    {
        public int CuentaID { get; set; }

        [Required(ErrorMessage = "El Cliente es obligatorio.")]
        public int ClienteID { get; set; }
        public string NombreCliente { get; set; }

        public string NumeroCedula { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "El Monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La Fecha de Vencimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El Estado es obligatorio.")]
        public string Estado { get; set; }
    }
}
