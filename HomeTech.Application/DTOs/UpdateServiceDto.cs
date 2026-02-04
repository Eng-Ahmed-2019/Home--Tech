namespace HomeTech.Application.DTOs
{
    public class UpdateServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool IsActive { get; set; }
        public decimal Price { get; set; } = default!;
    }
}