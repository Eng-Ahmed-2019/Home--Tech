using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class DeleteLocationCommandHandler
        : IRequestHandler<DeleteLocationCommand, bool>
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _audit;

        public DeleteLocationCommandHandler(ILocationRepository locationRepository,
            IHttpContextAccessor httpContextAccessor, IAuditService audit)
        {
            _locationRepository = locationRepository;
            _httpContextAccessor = httpContextAccessor;
            _audit = audit;
        }

        public async Task<bool> Handle(
        DeleteLocationCommand command,
        CancellationToken ct)
        {
            var location = await _locationRepository
                .GetByIdAsync(command.Dto.Id, ct);

            if (location == null)
                return false;

            await _locationRepository.SoftDeleteAsync(location);
            await _locationRepository.SaveChangesAsync(ct);

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
                ct);

            return true;
        }
    }
}