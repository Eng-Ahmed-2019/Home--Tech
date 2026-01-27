namespace HomeTech.Application.DTOs
{
    public record UpdateLocationDto(
        int Id,
        string Name,
        bool IsActive
    );
}