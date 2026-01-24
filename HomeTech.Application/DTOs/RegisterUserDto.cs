namespace HomeTech.Application.DTOs
{
    public record RegisterUserDto(
        string FullName,
        string Email,
        string Password
    );
}