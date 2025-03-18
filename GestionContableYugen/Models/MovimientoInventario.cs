using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionContableYugen.Models
{
    public class MovimientoInventario
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovimientoID { get; set; }

        public string CodigoBarras { get; set; }
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaMovimiento { get; set; }

        [ForeignKey("CodigoBarras")]
        public Producto Producto { get; set; }

    }
}
