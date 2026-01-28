using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class UpdateLocationCommandHandler
        : IRequestHandler<UpdateLocationCommand, bool>
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _audit;

        public UpdateLocationCommandHandler(
            IApplicationDbContext applicationDbContext,
            ILocationRepository locationRepository,
            IHttpContextAccessor httpContextAccessor,
            IAuditService audit
        )
        {
            _locationRepository = locationRepository;
            _httpContextAccessor = httpContextAccessor;
            _audit = audit;
        }

        public async Task<bool> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
        {
            var location = await _locationRepository.GetByIdAsync(command.Dto.Id, cancellationToken);
            if (location == null) return false;

            location.Name = command.Dto.Name;
            location.IsActive = command.Dto.IsActive;

            await _locationRepository.Update(location);
            await _locationRepository.SaveChangesAsync(cancellationToken);

            var UserId = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            var SessionId = _httpContextAccessor.HttpContext?
                .Items["SessionId"] as Guid?;

            await _audit.LogActionAsync(
                userId: UserId,
                sessionId: SessionId,
                action: "Updated Location",
                reason: $"Update location: {location.Name}",
                ipAddress: _httpContextAccessor.HttpContext!
                    .Connection.RemoteIpAddress!.ToString() ?? "UnKnown",
                cancellationToken);

            return true;
        }
    }
}