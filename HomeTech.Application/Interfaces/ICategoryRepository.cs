using HomeTech.Domain.Entities;

namespace HomeTech.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken cancellationToken);
        Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(Category category, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Category category);
        Task<bool> SoftDeleteAsync(Category category);
        Task SaveChangeAsync(CancellationToken cancellationToken);
        Task<List<Category>> GetAllAsync(CancellationToken cancellationToken);
    }
}