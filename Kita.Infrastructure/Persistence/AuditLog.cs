using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kita.Infrastructure.Persistence;

[Table("AuditLogs")]
public class AuditLog
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    public required string Action { get; set; }

    [Required]
    public required string EntityName { get; set; }

    // Retained for compatibility with existing integer-key lookups.
    public int EntityId { get; set; }

    // Supports string, GUID, and composite keys.
    public string? EntityKey { get; set; }

    [Required]
    public required string PerformedBy { get; set; }

    // Only approved audit metadata should be stored here.
    [Required]
    public required string Changes { get; set; }
}