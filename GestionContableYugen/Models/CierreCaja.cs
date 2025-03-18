using System;
using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class CierreCaja
    {
        [Key]
        public int CierreID { get; set; }  // Identificador del cierre

        [Required]
        public DateTime FechaCierre { get; set; } = DateTime.Now; // Fecha del cierre de caja

        [Required]
        public decimal MontoTotal { get; set; } // Monto total del cierre

        [Required]
        public int idCaja { get; set; } // ID de la caja asociada

        [Required]
        public decimal SaldoInicial { get; set; } // Saldo al inicio del día

        [Required]
        public decimal TotalIngresos { get; set; } // Suma de todos los ingresos del día

        [Required]
        public decimal TotalEgresos { get; set; } // Suma de todos los egresos del día

        [Required]
        public decimal SaldoFinal { get; set; } // Saldo después de restar ingresos y egresos
    }
}
