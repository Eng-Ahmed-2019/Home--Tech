namespace HomeTech.Application.DTOs
{
    public record ServicesDto(
        int id,
        string name,
        string description,
        decimal price,
        bool isActive
    );
}