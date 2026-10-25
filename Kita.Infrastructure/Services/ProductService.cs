using Kita.Application.Common.Interfaces;
using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using Kita.Application.Services;
using Kita.Domain.Abstractions;
using Kita.Domain.Inventory;
using Kita.Domain.Products;
using Kita.Domain.Specification;
using Kita.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Services
{
    public class ProductService: IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public ProductService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<Product?> GetProductAsync(int productId)
        {
            return await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
        }

        public async Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDTO)
        {
            var product = new Product
            {
                Name = productCreateDTO.Name,
                ProductCode = productCreateDTO.ProductCode,
                CostPrice = productCreateDTO.CostPrice,
                SellingPrice = productCreateDTO.SellingPrice,
                IsActive = productCreateDTO.IsActive,
            };

            product.SetCreated(DateTime.Now);

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var productDTO = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                IsActive = product.IsActive,
                LastUpdated = product.LastUpdated,
                DateAdded = product.DateAdded
            };

            return productDTO;
        }

        public async Task<bool> UpdateProductAsync(int productId, ProductUpdateDTO productUpdateDTO)
        {
            var product = await GetProductAsync(productId);
            if (product == null)
                return false;

            product.Name = productUpdateDTO.Name;
            product.CostPrice = productUpdateDTO.CostPrice;
            product.SellingPrice = productUpdateDTO.SellingPrice;
            product.IsActive = productUpdateDTO.IsActive;
            product.SetUpdated(DateTime.Now);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task UpdateStockAsync(InventoryLog inventoryLog)
        {
            Product? product = await _context.Products.FindAsync(inventoryLog.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product {inventoryLog.ProductId} not found.");

            if (inventoryLog.ChangeType == InventoryChangeType.StockIn)
                product.Stock += inventoryLog.Quantity;
            else if (inventoryLog.ChangeType == InventoryChangeType.StockOut)
            {
                if (product.Stock < inventoryLog.Quantity)
                    throw new InvalidOperationException($"{product.Name}: Not enough stock available.");
                product.Stock -= inventoryLog.Quantity;
            }
            else if (inventoryLog.ChangeType == InventoryChangeType.Adjustment)
                product.Stock = inventoryLog.Quantity;

            product.SetUpdated(DateTime.Now);

        }

        public async Task<bool> IsDupeProduct(Product product)
        {
            return await _unitOfWork.Repository<Product>().AnyAsync(
                e => e.Name == product.Name
                && e.CategoryId == product.CategoryId
                && e.SellingPrice == product.SellingPrice
                && e.CostPrice == product.CostPrice
                && e.Id != product.Id
            );
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _unitOfWork.Repository<Product>().AnyAsync(x => x.Id == id);
        }
    }
}
