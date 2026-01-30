namespace HomeTech.Application.DTOs
{
    public record ChangeEmailDto
    {
        public string NewEmail { get; init; } = default!;
    }
}