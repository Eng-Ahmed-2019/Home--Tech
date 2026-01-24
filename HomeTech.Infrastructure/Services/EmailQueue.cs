using System.Threading.Channels;
using HomeTech.Application.DTOs;
using HomeTech.Application.Interfaces;

namespace HomeTech.Infrastructure.Services
{
    public class EmailQueue : IEmailQueue
    {
        private readonly Channel<EmailJob> _channel;

        public EmailQueue()
        {
            _channel = Channel.CreateUnbounded<EmailJob>();
        }

        public async ValueTask EnqueueEmailAsync(EmailJob job)
        {
            await _channel.Writer.WriteAsync(job);
        }

        public async ValueTask<EmailJob> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}