using Kita.Domain.Products;

namespace Kita.Domain.Inventory
{
    public class InventoryLog
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public InventoryChangeType ChangeType { get; set; }
        public string? Remarks { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int OldStock { get; set; }
        public bool IsVoided { get; set; } = false;
        public DateTime? VoidedAt { get; set; }
        public string? VoidedByUserId { get; set; }
        public Product? Product { get; set; }
    }

    public enum InventoryChangeType
    {
        StockIn = 1,
        StockOut = 2,
        Adjustment = 3
    }
}
