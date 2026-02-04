using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class DeleteHomeTechServicesCommandHandler :
        IRequestHandler<DeleteHomeTechServicesCommand, bool>
    {
        private readonly IHomeTechServiceRepository _homeTechServiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _auditService;

        public DeleteHomeTechServicesCommandHandler(IHomeTechServiceRepository homeTechServiceRepository,
            IHttpContextAccessor httpContextAccessor, IAuditService auditService)
        {
            _homeTechServiceRepository = homeTechServiceRepository;
            _httpContextAccessor = httpContextAccessor;
            _auditService = auditService;
        }

        public async Task<bool> Handle(DeleteHomeTechServicesCommand command, CancellationToken cancellationToken)
        {
            var service = await _homeTechServiceRepository.GetByIdAsync(command.dto.Id, cancellationToken);
            if (service == null) { return false; }

            await _homeTechServiceRepository.SoftDeleteAsync(service, cancellationToken);
            await _homeTechServiceRepository.SaveChangesAsync(cancellationToken);

            var UserId = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            var SessionId = _httpContextAccessor.HttpContext?
                .Items["SessionId"] as Guid?;

            await _auditService.LogActionAsync(
                userId: UserId,
                sessionId: SessionId,
                action: "Delete Service",
                reason: $"Deleted Service: {service.Name.Trim()}",
                ipAddress: _httpContextAccessor.HttpContext!
                    .Connection.RemoteIpAddress!.ToString() ?? "UnKnown",
                cancellationToken
            );

            return true;
        }
    }
}