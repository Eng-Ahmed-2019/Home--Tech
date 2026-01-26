using HomeTech.Domain.Entities;
using HomeTech.Infrastructure.Data;
using HomeTech.Application.Interfaces;

namespace HomeTech.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly HomeTechDbContext _dbContext;

        public AuditService(HomeTechDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogActionAsync(
            string? userId,
            Guid? sessionId,
            string action,
            string reason,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            _dbContext.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                SessionId = sessionId,
                Action = action,
                Reason = reason,
                IpAddress = ipAddress
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}