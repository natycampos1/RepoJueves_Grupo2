using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class CambiarContrasenaPerfilModel
    {
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(250, MinimumLength = 8, ErrorMessage = "Mínimo 8 caracteres")]
        public string NuevaContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme la contraseña")]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}