
using System.ComponentModel.DataAnnotations;

namespace API_GRUPODOS.Models
{
  
    public class AgregarCatalogoSemanalRequestModel
    {
        [Required]
        public int IdProducto { get; set; }
        [Required]
        public DateTime FechaInicioSemana { get; set; }
        [Required]
        public int StockDisponible { get; set; }
        [Required]
        public int LimitePorPersona { get; set; }
    }
}