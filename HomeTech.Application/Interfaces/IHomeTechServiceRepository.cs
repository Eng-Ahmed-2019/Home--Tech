using HomeTech.Domain.Entities;

namespace HomeTech.Application.Interfaces
{
    public interface IHomeTechServiceRepository
    {
        Task AddAsync(HomeTechService service, CancellationToken ct);
        Task<bool> UpdateAsync(HomeTechService service, CancellationToken ct);
        Task<bool> SoftDeleteAsync(HomeTechService service, CancellationToken ct);
        Task<HomeTechService?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<HomeTechService>> GetAllAsync(CancellationToken ct);
        Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken ct);
    }
}