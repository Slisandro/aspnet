using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionContableYugen.Models
{
    public class Factura
    {
        [Key]
        public int FacturaID { get; set; }

        public int UsuarioID { get; set; }
        public int ClienteID { get; set; }
        public int idTipoPago { get; set; }
        public int idTipoFactura { get; set; } 
        public DateTime FechaFactura { get; set; }
        public decimal Total { get; set; }
        public decimal Impuesto { get; set; }
        public int? idDescuento { get; set; }
        public decimal MontoDescuento { get; set; }

        // Relaciones con otras tablas
        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; }

        [ForeignKey("ClienteID")]
        public Cliente Cliente { get; set; }

        [ForeignKey("idTipoPago")]
        public TipoPago TipoPago { get; set; }

        [ForeignKey("idTipoFactura")]
        public TipoFactura TipoFactura { get; set; }  

        [ForeignKey("idDescuento")]
        public Descuento Descuento { get; set; }

        public ICollection<DetalleFactura> Detalles { get; set; }
    }
}
