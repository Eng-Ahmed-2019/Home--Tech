using MediatR;
using HomeTech.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Interfaces;
using HomeTech.Application.Exceptions;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class ChangePasswordCommandHandler
        : IRequestHandler<ChangePasswordCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _audit;
        private readonly IHttpContextAccessor _http;
        private readonly IApplicationDbContext _context;

        public ChangePasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IAuditService audit,
            IHttpContextAccessor http,
            IApplicationDbContext context)
        {
            _userManager = userManager;
            _audit = audit;
            _http = http;
            _context = context;
        }

        public async Task<string> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new UserNotFoundException("المستخدم غير موجود");

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.Dto.CurrentPassword.Trim(),
                request.Dto.NewPassword.Trim()
            );
            if (!result.Succeeded)
                throw new PasswordException("كلمة المرور الحالية غير صحيحة");

            var sessions = await _context.UserSessions
                .Where(x => x.UserId == user.Id && !x.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var s in sessions) s.IsRevoked = true;

            await _context.SaveChangesAsync(cancellationToken);

            var SessionId = _http.HttpContext?
                .Items["SessionId"] as Guid?;

            await _audit.LogActionAsync(
                userId: user.Id,
                sessionId: SessionId,
                action: "Change Password",
                reason: "User changed password",
                ipAddress: _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                cancellationToken: cancellationToken
            );

            return "تم تغيير كلمة المرور وتسجيل الخروج من كل الأجهزة";
        }
    }
}