using HomeTech.Domain.Enums;

namespace HomeTech.Application.DTOs
{
    public class EmailJob
    {
        public string To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public int RetryCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public EmailTemplate Template { get; set; } = default!;
        public object Model { get; set; } = default!;
    }
}