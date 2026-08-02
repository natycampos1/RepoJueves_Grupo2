// PJ_GRUPODOS/Models/ProductoCatalogoModel.cs
namespace PJ_GRUPODOS.Models
{
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