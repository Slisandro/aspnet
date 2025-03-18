using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.ViewModels
{
    public class ProductoViewModel
    {
        public int ProductoID { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        public string NombreProducto { get; set; }

        [Required(ErrorMessage = "El código de barras es obligatorio")]
        public string CodigoBarras { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La cantidad disponible es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
        public int CantidadDisponible { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public string Categoria { get; set; }

        public string Descripcion { get; set; }

        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        // Campos para el cálculo del ROP
        public int DemandaDiariaPromedio { get; set; }
        public int TiempoEntregaProveedor { get; set; }
        public int StockSeguridad { get; set; }
    }
}
