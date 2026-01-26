namespace HomeTech.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UserId { get; set; }
        public Guid? SessionId { get; set; }
        public string Action { get; set; } = default!;
        public string Reason { get; set; } = default!;
        public string IpAddress { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}