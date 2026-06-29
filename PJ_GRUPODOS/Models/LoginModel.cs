using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}