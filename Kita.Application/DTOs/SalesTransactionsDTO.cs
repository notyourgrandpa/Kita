namespace Kita.Application.DTOs
{
    public class SalesTransactionsDTO
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string ProcessedByUserId { get; set; } = null!;
        public string ProcessedByUserName { get; set; } = null!;
        public DateTime Date { get; set; }

        public int CustomerId { get; set; }
    }
}
