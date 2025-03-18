using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class Cuenta
    {
        [Key]
        public int CuentaID { get; set; }
        public string CodigoCuenta { get; set; }
        public string Descripcion { get; set; }
        public string TipoCuenta { get; set; }
    }
}

