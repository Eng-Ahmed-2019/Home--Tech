using MediatR;
using System.Security.Claims;
using HomeTech.Domain.Entities;
using Microsoft.AspNetCore.Http;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class CreateCategoryCommandHandler :
        IRequestHandler<CreateCategoryCommand, string>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _auditService;

        public CreateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IHttpContextAccessor httpContextAccessor,
            IAuditService auditService
        )
        {
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _auditService = auditService;
        }

        public async Task<string> Handle(
            CreateCategoryCommand command,
            CancellationToken cancellationToken
        )
        {
            var category = new Category(command.Dto.name.Trim(), command.Dto.isActive);

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.SaveChangeAsync(cancellationToken);

            var UserId = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            var SessionId = _httpContextAccessor.HttpContext?
                .Items["SessionId"] as Guid?;

            await _auditService.LogActionAsync(
                userId: UserId,
                sessionId: SessionId,
                action: "Create Category",
                reason: $"Created Category: {category.Name.Trim()}",
                ipAddress: _httpContextAccessor.HttpContext!
                    .Connection.RemoteIpAddress!.ToString() ?? "UnKnown",
                cancellationToken
            );

            return "تم انشاء الفئة بنجاح";
        }
    }
}