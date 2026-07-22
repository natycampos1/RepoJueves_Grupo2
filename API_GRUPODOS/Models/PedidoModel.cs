namespace API_GRUPODOS.Models
{
    public class PedidoModel
    {
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public string TipoEntrega { get; set; } = string.Empty;
        public string? DireccionEntrega { get; set; }
        public string EstadoPedido { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}