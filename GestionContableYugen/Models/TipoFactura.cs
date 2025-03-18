using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class TipoFactura
    {
        [Key]
        public int idTipoFactura { get; set; }

        [Required]
        [StringLength(50)]
        public string Descripcion { get; set; }
    }
}
