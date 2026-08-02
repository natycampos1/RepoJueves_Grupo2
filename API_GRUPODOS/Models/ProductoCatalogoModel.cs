namespace API_GRUPODOS.Models
{
    // este modelo es solo para el catalogo que ve el cliente (RF-09)
    // trae los datos del producto + su configuracion de la semana (stock y limite)
    public class ProductoCatalogoModel
    {
        public int IdCatalogoSemanal { get; set; }
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Imagen { get; set; }
        public decimal Precio { get; set; }
        public bool PedidoAnticipado { get; set; }
        public int IdCategoria { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public int StockDisponible { get; set; }
        public int LimitePorPersona { get; set; }
    }
}