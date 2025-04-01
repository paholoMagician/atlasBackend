namespace AtlasERP.DTO
{
    public class ActualizarStockRequest
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public string TipoOperacion { get; set; }
    }
}
