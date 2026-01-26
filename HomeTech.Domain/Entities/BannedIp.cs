namespace HomeTech.Domain.Entities
{
    public class BannedIp
    {
        public int Id { get; set; }
        public string IpAddress { get; set; } = default!;
        public DateTime BannedUntil { get; set; }
    }
}