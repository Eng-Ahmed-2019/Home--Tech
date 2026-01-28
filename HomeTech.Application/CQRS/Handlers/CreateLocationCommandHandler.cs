using MediatR;
using System.Security.Claims;
using HomeTech.Domain.Entities;
using Microsoft.AspNetCore.Http;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class CreateLocationCommandHandler
        : IRequestHandler<CreateLocationCommand, string>
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _audit;

        public CreateLocationCommandHandler(ILocationRepository locationRepository,
            IApplicationDbContext _dbContext, IHttpContextAccessor httpContextAccessor,
            IAuditService audit)
        {
            _locationRepository = locationRepository;
            _httpContextAccessor = httpContextAccessor;
            _audit = audit;
        }

        public async Task<string> Handle(
            CreateLocationCommand request,
            CancellationToken ct)
        {
            var location = new Location
            {
                Name = request.Dto.Name,
                ParentId = request.Dto.ParentId
            };

            await _locationRepository.AddAsync(location, ct);
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
                action: "Create Location",
                reason: $"Created location: {location.Name}",
                ipAddress: _httpContextAccessor.HttpContext!
                    .Connection.RemoteIpAddress!.ToString() ?? "UnKnown",
                ct
            );

            return "تم انشاء الموقع بنجاح";
        }
    }
}