using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Persistence
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public required string Action { get; set; }
        public required string EntityName { get; set; }
        // Keep EntityId for legacy/int-based lookups, but store the canonical key in EntityKey
        public int EntityId { get; set; }
        public string? EntityKey { get; set; }
        public required string PerformedBy { get; set; }
        public required string Changes { get; set; }
    }
}
