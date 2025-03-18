using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Provincia
    {
        [Key]
        public int idProvincia { get; set; }
        public string NombreProvincia { get; set; }
    }
}
