using System.ComponentModel.DataAnnotations;

namespace API_GRUPODOS.Models
{
    public class InfoVariableSesionUsuarioModel
    {
        //El modelo de UsuarioRegistroModel esta compuesto de atributos de 3 tablas en la bd:
        //la tabla tbPersona, la tabla tbUsuario y la tabla tbTelefono.

        // --- Datos de tbPersona ---
        public string Identificacion { get; set; } = string.Empty;
        public int IdTipoIdentificacion { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public string? Direccion { get; set; }
        public string? Nacionalidad { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // --- Datos de tbTelefono ---
        public string NumTelefono { get; set; }

        // --- Datos de tbUsuario ---
        public string Email { get; set; } = string.Empty;
    }
}