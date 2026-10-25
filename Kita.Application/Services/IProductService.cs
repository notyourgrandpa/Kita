using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using Kita.Domain.Inventory;
using Kita.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Services
{
    public interface IProductService
    {
        Task<Product?> GetProductAsync(int productId);
        Task UpdateStockAsync(InventoryLog inventoryLog);
        Task<bool> IsDupeProduct(Product product);
        Task<bool> ProductExistsAsync(int productId);
    }
}
