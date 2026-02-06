using MediatR;
using HomeTech.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Exceptions;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class DeleteAccountCommandHandler :
        IRequestHandler<DeleteAccountCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _audit;
        private readonly IHttpContextAccessor _http;
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public DeleteAccountCommandHandler(
            UserManager<ApplicationUser> userManager,
            IAuditService audit,
            IHttpContextAccessor http,
            IApplicationDbContext context,
            IFileStorageService fileStorage
        )
        {
            _userManager = userManager;
            _audit = audit;
            _http = http;
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<string> Handle(
            DeleteAccountCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.userId)
                ?? throw new UserNotFoundException("المستخدم غير موجود");

            if (!user.IsActive) return "الحساب محذوف بالفعل";

            _fileStorage.Delete(user.ProfileImagePath);

            user.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            var sessions = await _context.UserSessions
                .Where(s => s.UserId == request.userId)
                .ToListAsync(cancellationToken);

            foreach (var s in sessions) s.IsRevoked = true;

            await _context.SaveChangesAsync(cancellationToken);

            await _audit.LogActionAsync(
                userId: user.Id,
                sessionId: request.SessionId,
                action: "Delete Account",
                reason: "User deactivated account",
                ipAddress: _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                cancellationToken: cancellationToken
            );

            return "تم حذف الحساب بنجاح";
        }
    }
}