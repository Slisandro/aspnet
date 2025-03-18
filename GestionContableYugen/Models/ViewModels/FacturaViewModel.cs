using System;
using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.ViewModels
{
    public class FacturaViewModel
    {
        public int FacturaID { get; set; }

        [Required]
        public int ClienteID { get; set; }
        public string ClienteNombre { get; set; }

        [Required]
        public int UsuarioID { get; set; }
        public string UsuarioNombre { get; set; }

        [Required]
        public int idTipoPago { get; set; }
        public string TipoPagoDescripcion { get; set; }

        public DateTime FechaFactura { get; set; }

        [Required]
        public decimal Total { get; set; }

        public int? idDescuento { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de factura.")]
        public int idTipoFactura { get; set; }
        public decimal MontoDescuento { get; set; }

        public List<DetalleFacturaViewModel> DetalleFacturas { get; set; } = new List<DetalleFacturaViewModel>();
    }
}