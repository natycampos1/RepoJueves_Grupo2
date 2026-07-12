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
        public string PrimerApellido { get; set; } = string.Empty;

        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public string? Direccion { get; set; }
        public string? Nacionalidad { get; set; }

        [Required]
        public string NumTelefono { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}