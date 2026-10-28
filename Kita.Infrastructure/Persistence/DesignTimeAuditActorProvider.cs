using Kita.Application.Common.Interfaces;

namespace Kita.Infrastructure.Persistence;

public sealed class DesignTimeAuditActorProvider : IAuditActorProvider
{
    public string ActorId => "System:EFMigration";
}