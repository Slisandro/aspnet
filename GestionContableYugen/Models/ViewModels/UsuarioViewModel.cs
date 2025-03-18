using AutoMapper;
using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;

namespace GestionContableYugen.ViewModels
{
    public class UsuarioViewModel
    {
        public int UsuarioID { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public bool Estado { get; set; }
        public int RolID { get; set; }
    }
}
