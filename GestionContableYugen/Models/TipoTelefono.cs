using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class TipoTelefono
    {
        [Key]
        public int idTipoTelefono { get; set; }
        public string Descripcion { get; set; }
    }
}
