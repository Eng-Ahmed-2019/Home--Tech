using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly IAuditService _audit;
    private readonly IHttpContextAccessor _http;

    public LogoutCommandHandler(IApplicationDbContext db, IAuditService audit, IHttpContextAccessor http)
    {
        _db = db;
        _audit = audit;
        _http = http;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken ct)
    {
        var session = await _db.UserSessions
            .FirstOrDefaultAsync(x => x.Id == request.SessionId, ct);

        if (session == null) return false;

        session.IsRevoked = true;
        await _db.SaveChangesAsync(ct);

        var ip = _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _audit.LogActionAsync(
            userId: session.UserId,
            sessionId: session.Id,
            action: "User Logout",
            reason: "User logged out successfully",
            ipAddress: ip,
            cancellationToken: ct
        );

        return true;
    }
}