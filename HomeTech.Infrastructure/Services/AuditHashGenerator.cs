using System.Text;
using HomeTech.Domain.Entities;
using System.Security.Cryptography;

namespace HomeTech.Infrastructure.Services
{
    public static class AuditHashGenerator
    {
        public static string Compute(
            AuditLog log,
            string? previousHash)
        {
            var raw = string.Join("|",
                log.UserId,
                log.SessionId,
                log.Action,
                log.Reason,
                log.IpAddress,
                log.CreatedAt.ToString("O"),
                previousHash
            );

            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));

            return Convert.ToHexString(bytes);
        }
    }
}