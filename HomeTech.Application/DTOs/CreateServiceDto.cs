namespace HomeTech.Application.DTOs
{
    public class CreateServiceDto
    {

        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}