using System.ComponentModel.DataAnnotations;

namespace PJ_GRUPODOS.Models
{
    public class UsuarioRegistroModel
    {
        //El modelo de UsuarioRegistroModel esta compuesto de atributos de 3 tablas en la bd: la tabla tbPersona, la tabla tbUsuario y la tabla tbTelefono.
        // --- Datos de tbPersona ---
        [Required(ErrorMessage = "La identificación es obligatoria")]
        [StringLength(20)]
        public string Identificacion { get; set; } = String.Empty;

        [Required(ErrorMessage = "El tipo de identificación es obligatorio")]
        public int IdTipoIdentificacion { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100)]
        public string NombreCompleto { get; set; } = String.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [StringLength(100)]
        public string PrimerApellido { get; set; } = String.Empty;

        [StringLength(100)]
        public string? SegundoApellido { get; set; } = String.Empty;

        [StringLength(10)]
        public string? Genero { get; set; }

        [StringLength(250)]
        public string? Direccion { get; set; }

        [StringLength(50)]
        public string? Nacionalidad { get; set; }

        [Required(ErrorMessage = "La fecha de registro es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // --- Datos de tbTelefono ---
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Número de teléfono no válido")]
        public string NumTelefono { get; set; } = string.Empty;

        // --- Datos de tbUsuario ---
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        [StringLength(100)]
        public string Email { get; set; } = String.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(250, MinimumLength = 8, ErrorMessage = "Mínimo 8 caracteres")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = String.Empty;

        [Required(ErrorMessage = "Confirme la contraseña")]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string ConfirmarContrasena { get; set; } = String.Empty;
    }
}
