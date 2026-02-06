using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;
using HomeTech.Application.Exceptions;
using HomeTech.Application.Interfaces;

namespace HomeTech.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SaveAsync(IFormFile? file, string folder)
        {
            if (file == null || file.Length == 0) return null;
            if (!file.ContentType.StartsWith("image/"))
                throw new ContentTypeException("ملف غير صالح، يجب أن يكون صورة فقط");
            if (file.Length > 2 * 1024 * 1024)
                throw new SizeFileException("حجم الصورة أكبر من 2MB");

            folder = folder.Trim();

            var uploads = Path.Combine(_env.WebRootPath, folder);
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var bytes = ms.ToArray();
            using var sha = SHA256.Create();
            var hash = Convert.ToHexString(sha.ComputeHash(bytes)).ToLower();

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{hash}{extension}";
            var path = Path.Combine(uploads, fileName);

            if (!File.Exists(path)) await File.WriteAllBytesAsync(path, bytes);

            return $"/{folder}/{fileName}";
        }

        public void Delete(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;

            var fullPath = Path.Combine(_env.WebRootPath, path.TrimStart('/'));

            if (File.Exists(fullPath.Trim())) File.Delete(fullPath.Trim());
        }
    }
}