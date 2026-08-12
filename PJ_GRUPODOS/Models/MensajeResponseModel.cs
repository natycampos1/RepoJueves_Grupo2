namespace PJ_GRUPODOS.Models
{
    public class MensajeResponseModel
    {
        public int IdMensaje { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
    }
}