using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Descuento
    {
        [Key]
        public int idDescuento { get; set; }

        [Required]
        public decimal Porcentaje { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
