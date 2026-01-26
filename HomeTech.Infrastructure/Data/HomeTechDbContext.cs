using HomeTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HomeTech.Infrastructure.Data
{
    public class HomeTechDbContext
        : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public HomeTechDbContext(DbContextOptions<HomeTechDbContext> options)
            : base(options)
        { }

        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<BannedIp> BannedIps => Set<BannedIp>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    }
}