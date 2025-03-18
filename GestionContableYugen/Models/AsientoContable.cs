using GestionContableYugen.Models;
using System.ComponentModel.DataAnnotations;

public class AsientoContable
{
    [Key]
    public int AsientoID { get; set; }

    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; }
    public decimal Debe { get; set; }
    public decimal Haber { get; set; }
    public int CuentaID { get; set; }

    public Cuenta Cuenta { get; set; }
}

