using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Canton
    {
        [Key]
        public int idCanton { get; set; }
        public string NombreCanton { get; set; }
        public int idProvincia { get; set; }

        public Provincia Provincia { get; set; }
    }
}
