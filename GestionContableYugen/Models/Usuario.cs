using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionContableYugen.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        [StringLength(255)]
        public string Contrasena { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;


        public int RolID { get; set; } // Clave foránea

        [ForeignKey("RolID")]
        public Rol Rol { get; set; }  // Relación con la tabla de roles
    }

}


