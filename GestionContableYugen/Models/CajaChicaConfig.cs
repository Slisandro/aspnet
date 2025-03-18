using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class CajaChicaConfig
    {
        [Key]
        public int ID { get; set; } = 1; 
        [Required]
        public decimal SaldoDisponible { get; set; }
    }
}
