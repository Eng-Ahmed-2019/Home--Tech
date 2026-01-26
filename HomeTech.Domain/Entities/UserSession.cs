namespace HomeTech.Domain.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public string IpAddress { get; set; } = default!;
    }
}