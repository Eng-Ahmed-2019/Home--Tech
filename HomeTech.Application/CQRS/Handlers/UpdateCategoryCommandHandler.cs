using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class UpdateCategoryCommandHandler :
        IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _auditService;

        public UpdateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IHttpContextAccessor httpContextAccessor,
            IAuditService auditService
        )
        {
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _auditService = auditService;
        }

        public async Task<bool> Handle(
            UpdateCategoryCommand command,
            CancellationToken cancellationToken
        )
        {
            var category = await _categoryRepository.GetByIdAsync(command.Dto.id, cancellationToken);
            if (category == null) return false;

            category.SetName(command.Dto.name);
            category.SetActive(command.Dto.isActive);

            await _categoryRepository.UpdateAsync(category);
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
                action: "Update Category",
                reason: $"Updated Category: {category.Name}",
                ipAddress: _httpContextAccessor.HttpContext!
                    .Connection.RemoteIpAddress!.ToString() ?? "UnKnown",
                cancellationToken
            );

            return true;
        }
    }
}