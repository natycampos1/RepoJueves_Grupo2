using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Ingrese su correo electrónico.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese su contraseña.")]
        public string Contrasenna { get; set; } = string.Empty;
    }
}