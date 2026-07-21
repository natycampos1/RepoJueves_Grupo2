namespace PJ_GRUPODOS.Models
{
    public class ProductoModel
    {
        public int IdProducto { get; set; }
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? Imagen { get; set; }

        public int Stock { get; set; }
    }
}