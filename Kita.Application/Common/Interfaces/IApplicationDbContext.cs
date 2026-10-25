using Kita.Domain.Categories;
using Kita.Domain.Customers;
using Kita.Domain.Expenses;
using Kita.Domain.Inventory;
using Kita.Domain.Products;
using Kita.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }
        DbSet<Category> Categories { get; }
        DbSet<SalesTransaction> SalesTransactions { get; }
        DbSet<InventoryLog> InventoryLogs { get; }
        DbSet<Expense> Expenses { get; }
        DbSet<Customer> Customers { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
