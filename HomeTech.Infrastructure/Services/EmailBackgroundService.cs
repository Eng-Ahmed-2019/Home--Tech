using HomeTech.Application.DTOs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HomeTech.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeTech.Infrastructure.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IEmailQueue _emailQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EmailBackgroundService> _logger;

        public EmailBackgroundService(
            IEmailQueue emailQueue,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<EmailBackgroundService> logger)
        {
            _emailQueue = emailQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                EmailJob? job = null;

                try
                {
                    job = await _emailQueue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (job != null)
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    await emailService.SendAsync(job.To, job.Subject, job.Body);
                }
            }
        }
    }
}