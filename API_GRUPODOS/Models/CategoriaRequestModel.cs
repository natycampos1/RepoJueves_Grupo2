using System.ComponentModel.DataAnnotations;

namespace API_GRUPODOS.Models
{
    public class CategoriaRequestModel
    {
        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Descripcion { get; set; } = string.Empty;
    }
}