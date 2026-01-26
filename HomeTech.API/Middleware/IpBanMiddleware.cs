using HomeTech.Domain.Entities;
using HomeTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Interfaces;

namespace HomeTech.API.Middleware
{
    public class IpBanMiddleware
    {
        private readonly RequestDelegate _next;

        public IpBanMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, HomeTechDbContext db, IAuditService auditService)
        {
            var ip = context.Connection.RemoteIpAddress!.ToString();

            var banned = await db.Set<BannedIp>()
               .AnyAsync(x =>
                   x.IpAddress == ip &&
                   x.BannedUntil > DateTime.UtcNow);
            if (banned)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("جهازك محظور حاليا");
                await auditService.LogActionAsync(
                    userId: null,
                    sessionId: null,
                    action: "AccessDenied",
                    reason: "Banned IP attempted access",
                    ipAddress: ip,
                    cancellationToken: CancellationToken.None
                );
                return;
            }

            await _next(context);
        }
    }
}