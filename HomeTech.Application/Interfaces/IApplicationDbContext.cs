using HomeTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeTech.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationUser> Users { get; }
        DbSet<UserSession> UserSessions { get; }
        DbSet<Location> Locations { get; }
        DbSet<Category> Categories { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<BannedIp> BannedIps { get; }
        DbSet<HomeTechService> HomeTechServices { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}