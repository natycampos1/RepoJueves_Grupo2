using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class MensajeContactoModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El asunto es obligatorio")]
        public string Asunto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje es obligatorio")]
        [StringLength(1000, ErrorMessage = "Máximo 1000 caracteres")]
        public string Mensaje { get; set; } = string.Empty;
    }
}
