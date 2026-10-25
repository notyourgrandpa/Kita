using Kita.Domain.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kita.Domain.Sales
{
    public class SalesTransactionItem
    {
        public int Id { get; set; }
        public int SalesTransactionId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }  // UnitPrice * Quantity
        public SalesTransaction? SalesTransaction { get; set; }
        public Product? Product { get; set; }
    }
}
