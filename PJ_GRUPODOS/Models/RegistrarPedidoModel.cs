using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class RegistrarPedidoModel
    {
        [Required(ErrorMessage = "Seleccione un tipo de entrega")]
        public int IdTipoEntrega { get; set; }

        public string? DireccionEntrega { get; set; }
    }
}