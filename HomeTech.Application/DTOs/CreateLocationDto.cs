namespace HomeTech.Application.DTOs
{
    public record CreateLocationDto(
        string Name,
        int? ParentId,
        bool isActive
    );
}