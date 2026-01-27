using HomeTech.Domain.Entities;
using System.Threading.Channels;

namespace HomeTech.Infrastructure.Services
{
    public class AuditQueue
    {
        public Channel<AuditLog> Queue { get; } =
            Channel.CreateUnbounded<AuditLog>();
    }
}