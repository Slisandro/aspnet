using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Impuesto
    {
        [Key]
        public int idImpuesto { get; set; }

        [Required]
        public decimal Porcentaje { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
