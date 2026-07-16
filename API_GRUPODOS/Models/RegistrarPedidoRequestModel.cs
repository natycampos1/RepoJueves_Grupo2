using System.ComponentModel.DataAnnotations;

namespace API_GRUPODOS.Models
{
    public class RegistrarPedidoRequestModel
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdTipoEntrega { get; set; }

        public string? DireccionEntrega { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "El carrito no puede estar vacío")]
        public List<ItemCarritoModel> Carrito { get; set; } = new();
    }
}