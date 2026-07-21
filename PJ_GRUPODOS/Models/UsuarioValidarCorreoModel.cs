namespace PJ_GRUPODOS.Models
{
    public class UsuarioValidarCorreoModel
    {
        public int IdUsuario { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}