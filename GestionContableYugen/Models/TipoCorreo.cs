using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class TipoCorreo
    {
        [Key]
        public int idTipoCorreo { get; set; }
        public string Descripcion { get; set; }
    }
}
