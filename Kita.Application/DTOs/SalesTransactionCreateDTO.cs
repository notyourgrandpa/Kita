namespace Kita.Application.DTOs
{
    public class SalesTransactionCreateDTO
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public List<SalesTransactionItemCreateInputDTO> Items { get; set; } = new();

    }
}
