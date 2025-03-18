using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Correo
    {
        [Key]
        public int idCorreo { get; set; }
        public string DireccionCorreo { get; set; }
        public int? PersonaID { get; set; }
    }
}

