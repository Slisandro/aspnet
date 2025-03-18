using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.ViewModels
{
    public class ProveedorViewModel
    {
        public int ProveedorID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "El nombre solo puede contener letras.")]
        public string NombreProveedor { get; set; }

        [Required(ErrorMessage = "El número de cédula es obligatorio.")]
        [RegularExpression(@"^\d{9,12}$", ErrorMessage = "El número de cédula debe contener entre 9 y 12 dígitos.")]
        public string NumeroCedula { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe contener exactamente 8 dígitos.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido.")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string Direccion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Estado { get; set; } = true;
    }
}
