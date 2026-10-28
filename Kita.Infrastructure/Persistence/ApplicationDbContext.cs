using Kita.Application.Common.Interfaces;
using Kita.Application.Services;
using Kita.Domain.Categories;
using Kita.Domain.Customers;
using Kita.Domain.Expenses;
using Kita.Domain.Inventory;
using Kita.Domain.Products;
using Kita.Domain.Sales;
using Kita.Infrastructure.Identity;
using Kita.Infrastructure.Persistence.Auditing;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kita.Infrastructure.Persistence
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        private readonly IAuditActorProvider _auditActorProvider;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IAuditActorProvider auditActorProvider) : base(options)
        {
            _auditActorProvider = auditActorProvider;
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<InventoryLog> InventoryLogs => Set<InventoryLog>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<SalesTransaction> SalesTransactions => Set<SalesTransaction>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var auditLogs = PrepareAuditLogs();

            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                DetachAuditLogs(auditLogs);
                throw;
            }
        }

        public override int SaveChanges()
        {
            var auditLogs = PrepareAuditLogs();

            try
            {
                return base.SaveChanges();
            }
            catch
            {
                DetachAuditLogs(auditLogs);
                throw;
            }
        }

        private List<AuditLog> PrepareAuditLogs()
        {
            ChangeTracker.DetectChanges();

            var auditLogs = new List<AuditLog>();

            var entries = ChangeTracker.Entries()
                .Where(e =>
                    AuditPolicy.ShouldAudit(e.Metadata.ClrType) &&
                    e.State is EntityState.Added
                        or EntityState.Modified
                        or EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                var changedProperties = entry.Properties
                    .Where(p =>
                        entry.State != EntityState.Modified ||
                        p.IsModified)
                    .Select(p => p.Metadata.Name)
                    .OrderBy(name => name)
                    .ToList();

                if (changedProperties.Count == 0)
                    continue;

                var primaryKey = entry.Metadata.FindPrimaryKey();

                string? entityKey = null;
                int entityId = 0;

                if (primaryKey != null)
                {
                    var keyProperties = primaryKey.Properties
                        .Select(p => entry.Property(p.Name))
                        .ToList();

                    // Do not record temporary/generated keys
                    // before they have been persisted.
                    if (keyProperties.All(p => !p.IsTemporary))
                    {
                        entityKey = string.Join(":",
                            keyProperties.Select(p =>
                                p.CurrentValue?.ToString() ?? "null"));

                        if (keyProperties.Count == 1 &&
                            keyProperties[0].CurrentValue is int id)
                        {
                            entityId = id;
                        }
                    }
                }

                auditLogs.Add(new AuditLog
                {
                    Date = DateTime.UtcNow,
                    Action = entry.State.ToString(),
                    EntityName = entry.Metadata.ClrType.Name,
                    EntityId = entityId,
                    EntityKey = entityKey,
                    PerformedBy = _auditActorProvider.ActorId,
                    Changes = string.Join(", ", changedProperties)
                });
            }

            if (auditLogs.Count > 0)
                AuditLogs.AddRange(auditLogs);

            return auditLogs;
        }

        private void DetachAuditLogs(IEnumerable<AuditLog> auditLogs)
        {
            foreach (var auditLog in auditLogs)
            {
                Entry(auditLog).State = EntityState.Detached;
            }
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