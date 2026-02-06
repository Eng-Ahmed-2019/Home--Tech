using Microsoft.AspNetCore.Http;

namespace HomeTech.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string?> SaveAsync(IFormFile? file, string folder);
        void Delete(string? path);
    }
}