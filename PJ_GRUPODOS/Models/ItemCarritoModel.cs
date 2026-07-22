namespace PJ_GRUPODOS.Models
{
    public class ItemCarritoModel
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string? Imagen { get; set; }
    }
}