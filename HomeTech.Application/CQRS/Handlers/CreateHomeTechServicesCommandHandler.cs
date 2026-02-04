using MediatR;
using System.Security.Claims;
using HomeTech.Domain.Entities;
using Microsoft.AspNetCore.Http;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class CreateHomeTechServicesCommandHandler :
        IRequestHandler<CreateHomeTechServiceCommand, string>
    {
        private readonly IHomeTechServiceRepository _homeTechServiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _auditService;

        public CreateHomeTechServicesCommandHandler(IHomeTechServiceRepository homeTechServiceRepository,
            IHttpContextAccessor httpContextAccessor, IAuditService auditService)
        {
            _homeTechServiceRepository = homeTechServiceRepository;
            _httpContextAccessor = httpContextAccessor;
            _auditService = auditService;
        }

        public async Task<string> Handle(CreateHomeTechServiceCommand command, CancellationToken cancellationToken)
        {
            var service = new HomeTechService
            {
                Name = command.Dto.Name.Trim(),
                Description = command.Dto.Description.Trim(),
                Price = command.Dto.Price,
                IsActive = command.Dto.IsActive
            };

            await _homeTechServiceRepository.AddAsync(service, cancellationToken);
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
                action: "Create Service",
                reason: $"Created Service: {service.Name.Trim()}",
                ipAddress: _httpContextAccessor.HttpContext!
                    .Connection.RemoteIpAddress!.ToString() ?? "UnKnown",
                cancellationToken
            );

            return "تم انشاء الخدمة بنجاح";
        }
    }
}