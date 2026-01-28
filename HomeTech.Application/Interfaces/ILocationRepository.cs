using HomeTech.Domain.Entities;
using HomeTech.Application.Common;

namespace HomeTech.Application.Interfaces
{
    public interface ILocationRepository
    {
        Task<bool> ExistsByNameAsync(string name, int? ignoreId = null, CancellationToken ct = default);
        Task<Location?> GetByIdAsync(int id, CancellationToken ct);
        Task AddAsync(Location location, CancellationToken ct);
        Task<bool> Update(Location location);
        Task<bool> SoftDeleteAsync(Location location);
        Task<PagedResult<Location>> GetMainAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<Location>> GetSubAsync(int parentId, int page, int pageSize, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task<List<Location>> GetAllActiveAsync(CancellationToken ct);
    }
}