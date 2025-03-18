using System;

namespace GestionContableYugen.ViewModels
{
    public class ReporteFacturacionViewModel
    {
        public int FacturaID { get; set; }
        public string ClienteNombre { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
    }
}
