namespace HomeTech.Application.DTOs
{
    public record ChangePasswordDto
    {
        public string CurrentPassword { get; init; } = default!;
        public string NewPassword { get; init; } = default!;
    }
}