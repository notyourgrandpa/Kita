using Kita.Domain.Inventory;

namespace Kita.Application.DTOs
{
    public class InventoryLogDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public InventoryChangeType ChangeType { get; set; }
        public string? Remarks { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; } = null!;
        public bool IsVoided { get; set; }
    }
}
