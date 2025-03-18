using GestionContableYugen.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Cliente
{
    [Key]
    public int ClienteID { get; set; }

    [Required]
    public string NombreCliente { get; set; }

    public string Telefono { get; set; }
    public string CorreoElectronico { get; set; }
    public string Direccion { get; set; }
    public DateTime FechaRegistro { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "El número de cédula debe tener un máximo de 20 caracteres.")]
    public string NumeroCedula { get; set; }

    public bool Estado { get; set; } = true;

}

