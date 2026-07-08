using System.ComponentModel.DataAnnotations;

namespace API_GRUPODOS.Models
{
    public class MensajeContactoRequestModel
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Asunto { get; set; } = string.Empty;

        [Required]
        public string Mensaje { get; set; } = string.Empty;
    }
}