using Kita.Domain.Inventory;

namespace Kita.Application.DTOs
{
    public class InventoryLogCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public InventoryChangeType ChangeType { get; set; }
        public string? Remarks { get; set; }
    }
}
