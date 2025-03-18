using System.ComponentModel.DataAnnotations;

namespace GestionContableYugen.Models
{
    public class ConceptoCajaChica
    {
        [Key]
        public int idConcepto { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
