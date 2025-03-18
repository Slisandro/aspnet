using System;
using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.ViewModels
{
    public class CierreCajaViewModel
    {
        public int CierreID { get; set; }

        [Required]
        public DateTime FechaCierre { get; set; } = DateTime.Now;

        [Required]
        public decimal SaldoInicial { get; set; }

        [Required]
        public decimal TotalIngresos { get; set; }

        [Required]
        public decimal TotalEgresos { get; set; }

        public decimal SaldoFinal { get; set; }
    }
}
