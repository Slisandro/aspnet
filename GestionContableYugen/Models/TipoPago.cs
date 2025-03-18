using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class TipoPago
    {
        [Key]
        public int idTipoPago { get; set; }

        [Required]
        public string Descripcion { get; set; }
    }
}
