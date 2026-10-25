using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Kita.Infrastructure.Identity;
using Kita.Domain.Categories;
using Kita.Domain.Expenses;
using Kita.Domain.Inventory;
using Kita.Domain.Products;
using Kita.Domain.Sales;
using Kita.Domain.Customers;
using Kita.Application.Common.Interfaces;
using Kita.Application.Services;

namespace Kita.Infrastructure.Persistence
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        private readonly ICurrentUserService? _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService? currentUserService = null) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<InventoryLog> InventoryLogs => Set<InventoryLog>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<SalesTransaction> SalesTransactions => Set<SalesTransaction>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = new List<AuditLog>();

            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted);

            foreach (var e in entries)
            {
                // Build a canonical string key from primary key properties (supports composite keys)
                var pk = e.Metadata.FindPrimaryKey();
                string? entityKey = null;
                int entityId = 0;

                if (pk != null)
                {
                    var keyParts = pk.Properties.Select(p =>
                    {
                        var prop = e.Property(p.Name);
                        var cv = prop.CurrentValue;
                        return cv?.ToString() ?? "null";
                    });

                    entityKey = string.Join(":", keyParts);

                    // If single int key, try to parse for EntityId convenience
                    if (pk.Properties.Count == 1)
                    {
                        var singleVal = pk.Properties[0];
                        var propVal = e.Property(singleVal.Name).CurrentValue;
                        if (propVal is int i)
                            entityId = i;
                        else if (propVal != null)
                            int.TryParse(propVal.ToString(), out entityId);
                    }
                }

                auditEntries.Add(new AuditLog
                {
                    Action = e.State.ToString(),
                    EntityName = e.Entity.GetType().Name,
                    EntityId = entityId,
                    EntityKey = entityKey,
                    PerformedBy = _currentUserService?.UserId ?? "Unknown",
                    Changes = string.Join(", ", e.Properties
                        .Select(p => $"{p.Metadata.Name}: {p.CurrentValue}"))
                });
            }

            AuditLogs.AddRange(auditEntries);
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    Name = "Walk-in",
                    ContactNumber = "None"
                }
            );
        }
    }
}