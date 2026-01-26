namespace HomeTech.Application.DTOs
{
    public record LoginResponseDto
    {
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
        public DateTime ExpiresAt { get; init; }
    }
}