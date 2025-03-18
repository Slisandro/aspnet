using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestionContableYugen.ViewModels
{
    public class CajaChicaViewModel
    {
        public int MovimientoID { get; set; }

        [Required(ErrorMessage = "El UsuarioID es obligatorio.")]
        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "El NombreUsuario es obligatorio.")]
        public string NombreUsuario { get; set; } = "Desconocido";

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaMovimiento { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(-9999999.99, 9999999.99, ErrorMessage = "El monto debe estar entre -9,999,999.99 y 9,999,999.99.")]
        public decimal Monto { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no debe superar los 200 caracteres.")]
        public string Descripcion { get; set; }

        [StringLength(50, ErrorMessage = "El número de factura no debe superar los 50 caracteres.")]
        public string NumeroFactura { get; set; }

        [StringLength(100, ErrorMessage = "El nombre del proveedor no debe superar los 100 caracteres.")]
        public string Proveedor { get; set; }

        public string FacturaAdjunta { get; set; } = "No Adjunta";

        public IFormFile? FacturaAdjuntaFile { get; set; }

        public bool Anulado { get; set; } = false;
    }
}
