using System.ComponentModel.DataAnnotations;

namespace API_GRUPODOS.Models
{
    public class RegistrarPedidoRequestModel
    {
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string NombreCliente { get; set; } = string.Empty;

        [Required]
        public int IdTipoEntrega { get; set; }

        public string? DireccionEntrega { get; set; }

        [Required]
        public DateTime FechaEntregaProgramada { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "El carrito no puede estar vacío")]
        public List<ItemCarritoModel> Carrito { get; set; } = new();
    }
}