namespace PJ_GRUPODOS.Models
{
    public class InfoVariableSesionUsuarioModel
    {
        public int IdUsuario { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public int IdTipoIdentificacion { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public string? Direccion { get; set; }
        public string? Nacionalidad { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string NumTelefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int IdRol { get; set; }
    }
}