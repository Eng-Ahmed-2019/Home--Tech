using HomeTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeTech.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<UserSession> UserSessions { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}