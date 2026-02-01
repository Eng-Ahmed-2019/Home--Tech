namespace HomeTech.Application.DTOs
{
    public class UpdateServiceDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool isActived { get; set; }
        public decimal Price { get; set; } = default!;
    }
}