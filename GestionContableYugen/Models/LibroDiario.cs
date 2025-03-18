using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class LibroDiario
    {
        [Key]
        public int idLibroDiario { get; set; }
        public DateTime Fecha { get; set; }
        public int AsientoContableID { get; set; }

        public AsientoContable AsientoContable { get; set; }
    }
}

