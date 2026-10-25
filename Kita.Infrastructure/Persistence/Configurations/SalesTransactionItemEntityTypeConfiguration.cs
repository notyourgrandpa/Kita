using Kita.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kita.Infrastructure.Persistence.Configurations
{
    public class SalesTransactionItemEntityTypeConfiguration : IEntityTypeConfiguration<SalesTransactionItem>
    {
        public void Configure(EntityTypeBuilder<SalesTransactionItem> builder)
        {
            builder.ToTable("SalesTransactionItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.SalesTransactionId);
            builder.Property(x => x.ProductId);
            builder.Property(x => x.Quantity);
            builder.Property(x => x.UnitPrice).HasColumnType("decimal(10,2)");
            builder.Property(x => x.Subtotal).HasColumnType("decimal(10,2)");
            builder
                .HasOne(x => x.SalesTransaction)
                .WithMany(x => x.SalesTransactionItems)
                .HasForeignKey(x => x.SalesTransactionId);
            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.SalesTransactionItems)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
