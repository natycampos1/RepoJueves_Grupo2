
namespace API_GRUPODOS.Models
{
  
    public class CatalogoSemanalAdminModel
    {
        public int IdCatalogoSemanal { get; set; }
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int StockDisponible { get; set; }
        public int LimitePorPersona { get; set; }
        public bool Activo { get; set; }
    }
}