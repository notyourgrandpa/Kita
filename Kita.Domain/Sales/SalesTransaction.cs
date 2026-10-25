using Kita.Domain.Customers;

namespace Kita.Domain.Sales {
    public class SalesTransaction
    {
        public int Id { get; set; }        
        public decimal TotalAmount { get; set; }
        public required string PaymentMethod { get; set; }
        public string ProcessedByUserId { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public ICollection<SalesTransactionItem> SalesTransactionItems { get; set; } = new List<SalesTransactionItem>();
        public bool IsVoided { get; set; } = false;
        public DateTime? VoidedAt { get; set; }
        public string? VoidedByUserId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
    }
}
