using Kita.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kita.Infrastructure.Persistence.Configurations
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
               .HasDefaultValue("Walk-in")
               .HasMaxLength(100);
            builder.Property(c => c.ContactNumber)
                   .HasMaxLength(20);
            builder.Property(c => c.Email)
                   .HasMaxLength(100);
            builder.Property(c => c.Address)
                   .HasMaxLength(250);
            builder.HasMany(c => c.SalesTransactions)
                .WithOne(t => t.Customer)
                .HasForeignKey(c => c.CustomerId);
        }
    }
}
