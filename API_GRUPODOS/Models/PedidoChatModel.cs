namespace API_GRUPODOS.Models
{
 
    public class PedidoChatModel
    {
        public int IdPedido { get; set; }
        public string NombreInterlocutor { get; set; } = string.Empty;
        public string EstadoPedido { get; set; } = string.Empty;
    }
}