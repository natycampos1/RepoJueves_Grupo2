using System.ComponentModel.DataAnnotations;

namespace API_GRUPODOS.Models
{
    public class UsuarioEdicionRequestModel
    {
        public string Identificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public string? Direccion { get; set; }
        public string? Nacionalidad { get; set; }
        public string NumTelefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [StringLength(250, MinimumLength = 8, ErrorMessage = "Mínimo 8 caracteres")]
        public string? NuevaContrasena { get; set; }

        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmarNuevaContrasena { get; set; }
    }
}