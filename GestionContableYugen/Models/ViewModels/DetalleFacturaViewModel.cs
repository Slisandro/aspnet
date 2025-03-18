using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.ViewModels
{
    public class DetalleFacturaViewModel
    {
        public int DetalleFacturaID { get; set; }
        public int FacturaID { get; set; }
        public string CodigoBarras { get; set; }

        [Required]
        public string NombreProducto { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Cantidad { get; set; }

        public decimal Total => Precio * Cantidad;
    }
}
