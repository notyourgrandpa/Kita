using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.Services;
using Kita.Domain.Abstractions;
using Kita.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Services
{
    public class InventoryLogService: IInventoryLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        public InventoryLogService(IUnitOfWork unitOfWork, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
        }

        public async Task<InventoryLogDTO> CreateInventoryLogAsync(InventoryLogCreateDto inventoryLogDto)
        {
            if (inventoryLogDto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var product = await _productService.GetProductAsync(inventoryLogDto.ProductId);

            InventoryLog inventoryLog = new()
            {
                ProductId = inventoryLogDto.ProductId,
                Quantity = inventoryLogDto.Quantity,
                ChangeType = inventoryLogDto.ChangeType,
                Remarks = inventoryLogDto.Remarks,
                OldStock = product?.Stock ?? 0,
                Date = DateTime.UtcNow
            };

            await _unitOfWork.Repository<InventoryLog>().AddAsync(inventoryLog);

            await _productService.UpdateStockAsync(inventoryLog);

            return new InventoryLogDTO
            {
                Id = inventoryLog.Id,
                ProductId = inventoryLog.ProductId,
                Quantity = inventoryLog.Quantity,
                ChangeType = inventoryLog.ChangeType,
                Remarks = inventoryLog.Remarks,
                Date = inventoryLog.Date
            };
        }
    }
}
