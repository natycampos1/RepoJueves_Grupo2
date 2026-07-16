namespace PJ_GRUPODOS.Models
{
    public class PedidoAdminModel
    {
        public int IdPedido { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public string TipoEntrega { get; set; } = string.Empty;
        public string EstadoPedido { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}