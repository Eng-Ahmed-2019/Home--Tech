namespace HomeTech.Application.DTOs
{
    public record UpdateCategoryDto(
        int id,
        string name,
        bool isActive
    );
}