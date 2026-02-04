using HomeTech.Application.CQRS.Commands;
using HomeTech.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomeTech.Application.CQRS.Handlers
{
    public class UpdateHomeTechServiceCommandHandler :
        IRequestHandler<UpdateHomeTechServicesCommand, bool>
    {
        private readonly IHomeTechServiceRepository _homeTechServiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _auditService;

        public UpdateHomeTechServiceCommandHandler(IHomeTechServiceRepository homeTechServiceRepository,
            IHttpContextAccessor httpContextAccessor, IAuditService auditService)
        {
            _homeTechServiceRepository = homeTechServiceRepository;
            _httpContextAccessor = httpContextAccessor;
            _auditService = auditService;
        }

        public async Task<bool> Handle(UpdateHomeTechServicesCommand command, CancellationToken cancellationToken)
        {
            var service = await _homeTechServiceRepository.GetByIdAsync(command.Dto.Id, cancellationToken);
            if (service == null) { return false; }

            service.Name = command.Dto.Name.Trim();
            service.Description = command.Dto.Description.Trim();
            service.Price = command.Dto.Price;
            service.IsActive = command.Dto.IsActive;

            await _homeTechServiceRepository.UpdateAsync(service, cancellationToken);
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
                action: "Update Service",
                reason: $"Updated Service: {service.Name.Trim()}",
                ipAddress: _httpContextAccessor.HttpContext!
                    .Connection.RemoteIpAddress!.ToString() ?? "UnKnown",
                cancellationToken
            );

            return true;
        }
    }
}