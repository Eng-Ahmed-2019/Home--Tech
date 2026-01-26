using System.Security.Claims;
using HomeTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Interfaces;

namespace HomeTech.API.Middleware
{
    public class SessionRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, HomeTechDbContext db, IAuditService auditService)
        {
            await _next(context);

            // نتأكد إن فيه User و Authenticated
            var user = context.User;
            if (user?.Identity?.IsAuthenticated != true) return;

            // نجيب sessionId من الـ Claims
            var sessionIdClaim = user.FindFirst("sessionId");
            if (sessionIdClaim is null) return;

            if (!Guid.TryParse(sessionIdClaim.Value, out var sessionId)) return;

            // نجيب session من DB (Tracking علشان نقدر نعمل Update)
            var session = await db.UserSessions
                .FirstOrDefaultAsync(x =>
                    x.Id == sessionId &&
                    !x.IsRevoked);
            if (session is null) return;

            // 🔐 فحص الـ IP Address
            var currentIp = context.Connection.RemoteIpAddress?.ToString();
            if (!string.Equals(session.IpAddress, currentIp, StringComparison.Ordinal))
            {
                // IP مختلف → نلغي الجلسة
                session.IsRevoked = true;
                await db.SaveChangesAsync();
                await auditService.LogActionAsync(
                    action: "Session Revoked",
                    reason: "IP Address changed",
                    ipAddress: currentIp ?? "Unknown",
                    sessionId: session.Id,
                    userId: session.UserId
                );
                return;
            }

            // لو التوكن قرب يخلص
            var expClaim = user.FindFirst(ClaimTypes.Expiration) ?? user.FindFirst("exp");
            if (expClaim is null) return;

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value)).UtcDateTime;
            if (expiresAt <= DateTime.UtcNow.AddMinutes(2))
            {
                context.Response.Headers["X-Token-Expired"] = "true";
            }
        }
    }
}