using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Rol
    {
        [Key]
        public int RolID { get; set; }

        [Required]
        public string NombreRol { get; set; }
    }
}

