using Kita.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kita.Application.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        //[JsonPropertyName("sellingPrice")]
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int Stock { get; set; }
        public int MinimumStock { get; set; }
        public bool IsActive { get; set; }
        public string UnitName { get; set; } = String.Empty;
        public UnitOfMeasure Unit { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
