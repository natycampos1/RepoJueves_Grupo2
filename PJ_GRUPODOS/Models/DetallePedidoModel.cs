namespace PJ_GRUPODOS.Models
{
    public class DetallePedidoModel
    {
        public int IdDetallePedido { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}