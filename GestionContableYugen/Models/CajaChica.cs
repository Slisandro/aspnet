using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionContableYugen.Models
{
    [Table("Caja_Chica")]
    public class CajaChica
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovimientoID { get; set; }

        public int UsuarioID { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public string NumeroFactura { get; set; }
        public string Proveedor { get; set; }
        public string FacturaAdjunta { get; set; }

        public Usuario Usuario { get; set; }


        public bool? Anulado { get; set; }

    }
}
