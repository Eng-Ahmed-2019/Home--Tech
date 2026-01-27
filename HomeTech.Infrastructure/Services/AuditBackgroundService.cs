using HomeTech.Infrastructure.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HomeTech.Infrastructure.Services
{
    public class AuditBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AuditQueue _queue;

        public AuditBackgroundService(
            IServiceScopeFactory scopeFactory,
            AuditQueue queue)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var reader = _queue.Queue.Reader;

            while (await reader.WaitToReadAsync(stoppingToken))
            {
                var log = await reader.ReadAsync(stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<HomeTechDbContext>();

                var lastHash = await db.AuditLogs
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => x.Hash)
                    .FirstOrDefaultAsync(stoppingToken);

                log.PreviousHash = lastHash;
                log.Hash = AuditHashGenerator.Compute(log, lastHash);

                db.AuditLogs.Add(log);
                await db.SaveChangesAsync(stoppingToken);
            }
        }
    }
}