using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class LibroMayor
    {
        [Key]
        public int idLibroMayor { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public DateTime FechaCorte { get; set; }
    }
}

