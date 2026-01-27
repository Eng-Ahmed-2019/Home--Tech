using HomeTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeTech.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<UserSession> UserSessions { get; }
        DbSet<Location> Locations { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<BannedIp> BannedIps { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}