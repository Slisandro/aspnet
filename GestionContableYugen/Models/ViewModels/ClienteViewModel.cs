using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.ViewModels
{
    public class ClienteViewModel
    {
        public int ClienteID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]{3,40}$", ErrorMessage = "Solo se permiten letras y espacios (mínimo 3, máximo 40 caracteres)")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "El número de cédula es obligatorio.")]
        [RegularExpression(@"^\d{9,12}$", ErrorMessage = "Ingrese un número de cédula válido.")]
        public string NumeroCedula { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^\d{8,12}$", ErrorMessage = "Debe contener entre 8 y 12 números")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(100, ErrorMessage = "La dirección no puede superar los 100 caracteres")]
        public string Direccion { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

    }
}
