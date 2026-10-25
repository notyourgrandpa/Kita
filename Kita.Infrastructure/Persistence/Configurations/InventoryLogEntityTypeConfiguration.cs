using Kita.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Persistence.Configurations
{
    public class InventoryLogEntityTypeConfiguration : IEntityTypeConfiguration<InventoryLog>
    {
        public void Configure(EntityTypeBuilder<InventoryLog> builder)
        {
            builder.ToTable("InventoryLogs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.ProductId);
            builder.Property(x => x.Quantity);
            builder.Property(x => x.ChangeType);
            builder.Property(x => x.Remarks);
            builder.Property(x => x.Date);
            builder.Property(x => x.OldStock);
            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.InventoryLogs)
                .HasForeignKey(x => x.ProductId);
            //builder
            //    .HasOne(x => x.VoidedByUser)
            //    .WithMany(x => x.InventoryLogs)
            //    .HasForeignKey(x => x.VoidedByUserId);
        }
    }
}
