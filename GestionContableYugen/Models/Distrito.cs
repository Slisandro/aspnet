using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Distrito
    {
        [Key]
        public int idDistrito { get; set; }
        public string NombreDistrito { get; set; }
        public int idCanton { get; set; }

        public Canton Canton { get; set; }
    }
}
