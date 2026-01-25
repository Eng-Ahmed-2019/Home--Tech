using HomeTech.Domain.Enums;

namespace HomeTech.Application.Interfaces
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(
            EmailTemplate template,
            object model
        );
    }
}