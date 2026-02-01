using HomeTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Interfaces;

namespace HomeTech.Infrastructure.Services
{
    public class HomeTechServiceRepository : IHomeTechServiceRepository
    {
        private readonly IApplicationDbContext _context;

        public HomeTechServiceRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            int? excludeId,
            CancellationToken cancellationToken
        )
        {
            name = name.Trim();

            return await _context.HomeTechServices
                .AnyAsync(x => x.Name == name &&
                (!excludeId.HasValue || x.Id != excludeId), cancellationToken);
        }

        public async Task AddAsync(HomeTechService service, CancellationToken ct)
            => await _context.HomeTechServices.AddAsync(service, ct);

        public async Task<HomeTechService?> GetByIdAsync(int id, CancellationToken ct)
            => await _context.HomeTechServices.FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<bool> UpdateAsync(HomeTechService service, CancellationToken ct)
        {
            if (service == null) return Task.FromResult(false);

            service.MarkUpdated();
            _context.SaveChangesAsync(ct);

            return Task.FromResult(true);
        }

        public Task<bool> SoftDeleteAsync(HomeTechService service, CancellationToken ct)
        {
            if (service == null) return Task.FromResult(false);

            service.SoftDelete();

            return Task.FromResult(true);
        }

        public async Task<List<HomeTechService>> GetAllAsync(CancellationToken ct)
        {
            return await _context.HomeTechServices
                .Where(x => x.IsActive)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct);
    }
}