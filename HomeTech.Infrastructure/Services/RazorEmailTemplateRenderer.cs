using RazorLight;
using HomeTech.Domain.Enums;
using HomeTech.Application.Interfaces;

namespace HomeTech.Infrastructure.Services
{
    public class RazorEmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly RazorLightEngine _engine;

        public RazorEmailTemplateRenderer()
        {
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "EmailTemplates"
                    )
                )
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderAsync(
            EmailTemplate template,
            object model)
        {
            var templateName = template switch
            {
                EmailTemplate.ConfirmEmail => "ConfirmEmail.cshtml",
                _ => throw new ArgumentOutOfRangeException()
            };

            return await _engine.CompileRenderAsync(
                templateName,
                model
            );
        }
    }
}