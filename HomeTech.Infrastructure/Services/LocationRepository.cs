using HomeTech.Domain.Entities;
using HomeTech.Application.Common;
using HomeTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Interfaces;

namespace HomeTech.Infrastructure.Services
{
    public class LocationRepository : ILocationRepository
    {
        private readonly HomeTechDbContext _context;

        public LocationRepository(HomeTechDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            int? ignoreId,
            CancellationToken ct)
        {
            return await _context.Locations
                .AnyAsync(x =>
                    x.Name == name &&
                    (!ignoreId.HasValue || x.Id != ignoreId),
                    ct);
        }

        public async Task<Location?> GetByIdAsync(int id, CancellationToken ct)
            => await _context.Locations.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task AddAsync(Location location, CancellationToken ct)
            => await _context.Locations.AddAsync(location, ct);

        public void Update(Location location)
            => _context.Locations.Update(location);

        public Task SoftDeleteAsync(Location location)
        {
            location.IsDeleted = true;
            location.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public async Task<PagedResult<Location>> GetMainAsync(
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = _context.Locations
                .AsNoTracking()
                .Where(x => x.ParentId == null);

            return new PagedResult<Location>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = await query.CountAsync(ct),
                Items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct)
            };
        }

        public async Task<PagedResult<Location>> GetSubAsync(
            int parentId,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = _context.Locations
                .AsNoTracking()
                .Where(x => x.ParentId == parentId);

            return new PagedResult<Location>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = await query.CountAsync(ct),
                Items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct)
            };
        }

        public async Task SaveChangesAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct);

        public async Task<List<Location>> GetAllActiveAsync(CancellationToken ct)
        {
            return await _context.Locations
                .Where(x => x.IsActive)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}