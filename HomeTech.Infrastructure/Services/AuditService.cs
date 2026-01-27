using HomeTech.Domain.Entities;
using HomeTech.Application.Interfaces;

namespace HomeTech.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly AuditQueue _queue;

        public AuditService(AuditQueue queue)
        {
            _queue = queue;
        }

        public async Task LogActionAsync(
            string? userId,
            Guid? sessionId,
            string action,
            string reason,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            var log = new AuditLog
            {
                UserId = userId,
                SessionId = sessionId,
                Action = action,
                Reason = reason,
                IpAddress = ipAddress
            };

            await _queue.Queue.Writer.WriteAsync(log, cancellationToken);
        }
    }
}