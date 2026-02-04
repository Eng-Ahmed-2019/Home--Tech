using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTech.Domain.Entities
{
    public class HomeTechService : BaseEntity
    {
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; }
        public string Description { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = default!;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}