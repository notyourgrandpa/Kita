using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Persistence.Configurations
{
    public class AuditLogEntityTypeConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Date);
            builder.Property(x => x.Action).IsRequired();
            builder.Property(x => x.EntityName).IsRequired();
            builder.Property(x => x.EntityId);
            builder.Property(x => x.EntityKey).HasMaxLength(500).IsUnicode(false);
            builder.Property(x => x.PerformedBy).IsRequired();
            builder.Property(x => x.Changes).IsRequired();
        }
    }
}
