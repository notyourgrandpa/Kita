using Kita.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kita.Infrastructure.Persistence.Configurations
{
    public class SalesTransactionEntityTypeConfiguration : IEntityTypeConfiguration<SalesTransaction>
    {
        public void Configure(EntityTypeBuilder<SalesTransaction> builder)
        {
            builder.ToTable("SalesTransactions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.TotalAmount).HasColumnType("decimal(10,2)");
            builder.Property(x => x.PaymentMethod).IsRequired();
            builder.Property(x => x.ProcessedByUserId).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.IsVoided).HasDefaultValue(false);
            builder.HasOne(x => x.Customer)
                .WithMany(s => s.SalesTransactions)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
