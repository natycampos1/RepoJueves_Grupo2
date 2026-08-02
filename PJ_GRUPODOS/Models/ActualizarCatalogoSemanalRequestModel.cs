using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class ActualizarCatalogoSemanalRequestModel
    {
        [Required]
        public int StockDisponible { get; set; }
        [Required]
        public int LimitePorPersona { get; set; }
        [Required]
        public bool Activo { get; set; }
    }
}