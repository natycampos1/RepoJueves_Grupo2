namespace PJ_GRUPODOS.Models
{
    public class UsuarioConsultaModel
    {
        public string Identificacion { get; set; } = string.Empty;
        public string TipoIdentificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public string? Direccion { get; set; }
        public string? Nacionalidad { get; set; }
        public string NumTelefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
