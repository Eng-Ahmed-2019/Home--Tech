using HomeTech.Domain.Entities;
using HomeTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Interfaces;

namespace HomeTech.Infrastructure.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly HomeTechDbContext _context;

        public CategoryRepository(HomeTechDbContext context)
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

            return await _context.Categories
                .AnyAsync(
                    x => x.Name == name &&
                    (!excludeId.HasValue || x.Id != excludeId),
                    cancellationToken
                );
        }

        public async Task<Category?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken
        ) => await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(Category category, CancellationToken cancellationToken)
            => await _context.Categories.AddAsync(category, cancellationToken);

        public Task<bool> UpdateAsync(Category category)
        {
            if (category == null) return Task.FromResult(false);

            category.MarkUpdated();
            _context.Categories.Update(category);

            return Task.FromResult(true);
        }

        public Task<bool> SoftDeleteAsync(Category category)
        {
            if (category == null) return Task.FromResult(false);

            category.SoftDelete();
            return Task.FromResult(true);
        }

        public async Task SaveChangeAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);

        public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}