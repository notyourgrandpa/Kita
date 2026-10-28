using Kita.Application.Common.Interfaces;
using Kita.Application.Services;

namespace Kita.Server.Services;

public sealed class HttpAuditActorProvider : IAuditActorProvider
{
    private readonly ICurrentUserService _currentUserService;

    public HttpAuditActorProvider(
        ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public string ActorId =>
        _currentUserService.UserId
        ?? "Unauthenticated";
}