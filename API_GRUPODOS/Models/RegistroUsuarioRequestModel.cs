using System.ComponentModel.DataAnnotations;

namespace API_GRUPODOS.Models
{
    public class RegistroUsuarioRequestModel
    {
        //El modelo de UsuarioRegistroModel esta compuesto de atributos de 3 tablas en la bd:
        //la tabla tbPersona, la tabla tbUsuario y la tabla tbTelefono.

        // --- Datos de tbPersona ---
        public string Identificacion { get; set; }
        public int IdTipoIdentificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public string? Direccion { get; set; }
        public string? Nacionalidad { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // --- Datos de tbTelefono ---
        public string NumTelefono { get; set; }

        // --- Datos de tbUsuario ---
        public string Email { get; set; }
        public string Contrasena { get; set; }
        public string ConfirmarContrasena { get; set; }

        public int IdRol { get; set; }
    }
}