using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionContableYugen.Models
{
    public class Inventario
    {
        [Key]
        public int ProductoID { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreProducto { get; set; }

        [Required]
        public int CategoriaID { get; set; }  // ← Puedes dejar solo la ID si necesitas categorizar pero sin relación

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal Precio { get; set; }
    }
}

