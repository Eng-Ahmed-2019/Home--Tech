using System.Security.Claims;
using HomeTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace HomeTech.API.Middleware
{
    public class SessionRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public SessionRefreshMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(
            HttpContext context,
            HomeTechDbContext db,
            IAuditService auditService,
            ITokenReplayService replayService)
        {
            // 🔹 لو مش authenticated نكمل عادي
            var user = context.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var jti = user.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ?? JwtRegisteredClaimNames.Jti;
            if (await replayService.IsJtiUsedAsync(jti))
            {
                context.Response.StatusCode = 401;
                return;
            }

            var tokenLifetime = TimeSpan.FromMinutes(
                _config.GetValue<int>("Jwt:LifetimeMinutes"));
            await replayService.MarkJtiAsUsedAsync(jti, tokenLifetime);

            // 🔹 UserId
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                await _next(context);
                return;
            }

            // 🔹 SessionId
            var sessionIdClaim = user.FindFirst("sessionId");
            if (sessionIdClaim is null ||
                !Guid.TryParse(sessionIdClaim.Value, out var sessionId))
            {
                await _next(context);
                return;
            }

            // 🔹 خزنهم في HttpContext.Items ⭐
            context.Items["UserId"] = userId;
            context.Items["SessionId"] = sessionId;
            context.Items["IpAddress"] =
                context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // 🔹 نجيب Session من DB
            var session = await db.UserSessions
                .FirstOrDefaultAsync(x =>
                    x.Id == sessionId &&
                    !x.IsRevoked);

            if (session is null)
            {
                await _next(context);
                return;
            }

            // 🔐 فحص IP
            var currentIp = context.Items["IpAddress"]!.ToString() ?? "Unknown";
            if (!string.Equals(session.IpAddress, currentIp, StringComparison.Ordinal))
            {
                session.IsRevoked = true;
                await db.SaveChangesAsync(CancellationToken.None);

                await auditService.LogActionAsync(
                    userId: session.UserId,
                    sessionId: session.Id,
                    action: "Session Revoked",
                    reason: "IP Address changed",
                    ipAddress: currentIp
                );

                return;
            }

            var ipClaim = user.FindFirst("ip")?.Value;
            if (ipClaim != currentIp)
            {
                session.IsRevoked = true;
                await db.SaveChangesAsync(CancellationToken.None);
                return;
            }

            // 🔹 فحص انتهاء التوكن
            var expClaim = user.FindFirst(ClaimTypes.Expiration)
                           ?? user.FindFirst("exp");

            if (expClaim != null &&
                long.TryParse(expClaim.Value, out var exp))
            {
                var expiresAt = DateTimeOffset
                    .FromUnixTimeSeconds(exp)
                    .UtcDateTime;

                if (expiresAt <= DateTime.UtcNow.AddMinutes(2))
                {
                    context.Response.Headers["X-Token-Expired"] = "true";
                }
            }

            await _next(context);
        }
    }
}