using System.ComponentModel.DataAnnotations;

namespace API_GRUPODOS.Models
{
    public class PerfilRequestModel
    {
        [Required]
        public string Identificacion { get; set; } = string.Empty;

        [Required]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        public string NumTelefono { get; set; } = string.Empty;
    }
}