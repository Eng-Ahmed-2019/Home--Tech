namespace HomeTech.Domain.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int? ParentId { get; set; }
        public Location? Parent { get; set; }
        public ICollection<Location> SubLocations { get; set; }
            = new List<Location>();
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}