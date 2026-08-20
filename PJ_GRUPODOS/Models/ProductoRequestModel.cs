using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class ProductoRequestModel
    {
        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(250, ErrorMessage = "Máximo 250 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        public string? Imagen { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, 999999, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Los puntos de esfuerzo son obligatorios")]
        [Range(0, 999, ErrorMessage = "Los puntos de esfuerzo no pueden ser negativos")]
        public int PuntosEsfuerzo { get; set; }

        public bool PedidoAnticipado { get; set; }
    }
}