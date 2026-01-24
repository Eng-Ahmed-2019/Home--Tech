using HomeTech.Application.DTOs;

namespace HomeTech.Application.Interfaces
{
    public interface IEmailQueue
    {
        ValueTask EnqueueEmailAsync(EmailJob job);
        ValueTask<EmailJob> DequeueAsync(CancellationToken cancellationToken);
    }
}