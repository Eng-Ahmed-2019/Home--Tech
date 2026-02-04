using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class DeleteCategoryCommandHandler :
        IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _auditService;

        public DeleteCategoryCommandHandler(
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
            DeleteCategoryCommand command,
            CancellationToken cancellationToken
        )
        {
            var category = await _categoryRepository.GetByIdAsync(command.Dto.id, cancellationToken);
            if (category == null) return false;

            await _categoryRepository.SoftDeleteAsync(category);
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
                action: "Delete Category",
                reason: $"Deleted Category: {category.Name.Trim()}",
                ipAddress: _httpContextAccessor.HttpContext!
                    .Connection.RemoteIpAddress!.ToString() ?? "UnKnown",
                cancellationToken
            );

            return true;
        }
    }
}