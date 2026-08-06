using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class RegistrarPedidoModel
    {
        [Required(ErrorMessage = "Seleccione un tipo de entrega")]
        public int IdTipoEntrega { get; set; }

        public string? DireccionEntrega { get; set; }

        [Required(ErrorMessage = "Seleccione la fecha de entrega")]
        public DateTime FechaEntrega { get; set; }

        [Required(ErrorMessage = "Seleccione la hora de entrega")]
        public string HoraEntrega { get; set; } = string.Empty;
    }
}