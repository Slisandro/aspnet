using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionContableYugen.Models
{
    public class DetalleFactura
    {
        [Key]
        public int DetalleFacturaID { get; set; }

        [Required]
        public int FacturaID { get; set; }

        [ForeignKey("FacturaID")]
        public Factura Factura { get; set; }

        [Required]
        public string CodigoBarras { get; set; }

        [ForeignKey("CodigoBarras")]
        public Producto Producto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal Precio { get; set; }

        
        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string NombreProducto { get; set; }

        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
    }
}
