using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Producto
    {
        [Key]
        public string CodigoBarras { get; set; }

        public string NombreProducto { get; set; }
        public decimal Precio { get; set; }
        public int CantidadDisponible { get; set; }
        public string Categoria { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaIngreso { get; set; }

        // Campos para el cálculo del ROP
        public int DemandaDiariaPromedio { get; set; }
        public int TiempoEntregaProveedor { get; set; }
        public int StockSeguridad { get; set; }
    }
}
