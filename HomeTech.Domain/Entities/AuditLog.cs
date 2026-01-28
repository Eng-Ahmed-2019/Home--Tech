namespace HomeTech.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public string? UserId { get; set; }
        public Guid? SessionId { get; set; }
        public string Action { get; set; } = default!;
        public string Reason { get; set; } = default!;
        public string IpAddress { get; set; } = default!;
        public string Hash { get; set; } = default!;
        public string? PreviousHash { get; set; }
    }
}