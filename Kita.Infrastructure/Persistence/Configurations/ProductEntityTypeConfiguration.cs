using Kita.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Persistence.Configurations
{
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.CategoryId);
            builder.Property(x => x.CostPrice).HasColumnType("decimal(10,2)");
            builder.Property(x => x.SellingPrice).HasColumnType("decimal(10,2)");
            builder.Property(x => x.IsActive);
            builder.Property(x => x.DateAdded);
            builder.Property(x => x.Unit);
            builder.Property(x => x.LastUpdated);
            builder.Property(x => x.Stock);
            builder.Property(x => x.MinimumStock);
            builder.Property(x => x.ProductCode).IsRequired();
            builder
                .HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId);
        }
    }
}
