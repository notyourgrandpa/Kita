
using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Domain.Inventory;

namespace Kita.Application.Services
{
    public interface IInventoryLogService
    {
        Task<InventoryLogDTO> CreateInventoryLogAsync(InventoryLogCreateDto inventoryLogDto);
    }
}
