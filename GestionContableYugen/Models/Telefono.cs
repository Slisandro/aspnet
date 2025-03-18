using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Telefono
    {
        [Key]
        public int idTelefono { get; set; }
        public string NumeroTelefono { get; set; }
        public string TipoTelefono { get; set; }
        public int? PersonaID { get; set; }
    }
}

