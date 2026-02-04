using System.ComponentModel.DataAnnotations;

namespace HomeTech.Domain.Entities
{
    public class Location : BaseEntity
    {
        public string Name { get; set; } = default!;
        public int? ParentId { get; set; }
        public Location? Parent { get; set; }
        public ICollection<Location> SubLocations { get; set; }
            = new List<Location>();
        public bool IsActive { get; set; } = true;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}