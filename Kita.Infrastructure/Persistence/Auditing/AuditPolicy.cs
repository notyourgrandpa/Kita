using Kita.Domain.Categories;
using Kita.Domain.Products;
using Kita.Domain.Inventory;
using Kita.Domain.Sales;
using Kita.Domain.Expenses;

namespace Kita.Infrastructure.Persistence.Auditing;

public static class AuditPolicy
{
    private static readonly HashSet<Type> AuditedEntities =
    [
        typeof(Product),
        typeof(Category),
        typeof(InventoryLog),
        typeof(SalesTransaction),
        typeof(SalesTransactionItem),
        typeof(Expense)
    ];

    public static bool ShouldAudit(Type entityType)
    {
        return AuditedEntities.Contains(entityType);
    }
}