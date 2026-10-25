namespace Kita.Application.DTOs
{
    public class SalesTransactionItemCreateInputDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
