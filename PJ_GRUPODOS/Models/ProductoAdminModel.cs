namespace PJ_GRUPODOS.Models
{
    public class ProductoAdminModel
    {
        public int IdProducto { get; set; }
        public int IdCategoria { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? Imagen { get; set; }
    }
}