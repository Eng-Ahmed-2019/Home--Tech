using HomeTech.Application.DTOs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HomeTech.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeTech.Infrastructure.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private const int MaxRetryCount = 3;
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
                EmailJob job;

                try
                {
                    job = await _emailQueue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var renderer = scope.ServiceProvider.GetRequiredService<IEmailTemplateRenderer>();
                    var htmlBody = await renderer.RenderAsync(
                        job.Template,
                        job.Model
                    );

                    await emailService.SendAsync(job.To, job.Subject, htmlBody);

                    _logger.LogInformation(
                        "Email sent successfully to {Email}",
                        job.To
                    );
                }
                catch (Exception ex)
                {
                    job.RetryCount++;

                    _logger.LogError(
                        ex,
                        "Failed to send email to {Email}. Retry {Retry}/{MaxRetry}",
                        job.To,
                        job.RetryCount,
                        MaxRetryCount
                    );

                    if (job.RetryCount < MaxRetryCount)
                    {
                        await _emailQueue.EnqueueEmailAsync(job);
                    }
                    else
                    {
                        _logger.LogCritical(
                            "Email permanently failed for {Email}",
                            job.To
                        );
                    }
                }
            }
        }
    }
}