using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class RecuperarAccesoModel
    {
        [Required(ErrorMessage = "Ingrese su correo electrónico.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string CorreoElectronico { get; set; } = string.Empty;
    }
}