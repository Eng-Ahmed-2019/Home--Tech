using MediatR;
using System.Text;
using HomeTech.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Interfaces;
using HomeTech.Application.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class ConfirmEmailCommandHandler
        : IRequestHandler<ConfirmEmailCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _audit;
        private readonly IHttpContextAccessor _http;

        public ConfirmEmailCommandHandler(
            UserManager<ApplicationUser> userManager,
            IAuditService audit,
            IHttpContextAccessor http)
        {
            _userManager = userManager;
            _audit = audit;
            _http = http;
        }

        public async Task<string> Handle(
            ConfirmEmailCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new EmailNotFoundException("المستخدم غير موجود");

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(request.Token))
                ?? throw new TokenNotFoundException("رابط التأكيد غير صالح أو منتهي");

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
                throw new TokenNotFoundException("رابط التأكيد غير صالح أو منتهي");

            if (!string.IsNullOrEmpty(user.PendingEmail))
            {
                user.Email = user.PendingEmail;
                user.UserName = user.PendingEmail;
                user.PendingEmail = null;
                user.EmailConfirmed = true;

                await _userManager.UpdateAsync(user);
            }

            if (!user.IsActive)
            {
                user.IsActive = true;
                await _userManager.UpdateAsync(user);
            }

            var SessionId = _http.HttpContext?
                .Items["SessionId"] as Guid?;

            await _audit.LogActionAsync(
                userId: user.Id,
                sessionId: SessionId,
                action: "Confirm Email",
                reason: "Email confirmed successfully",
                ipAddress: _http.HttpContext!
                    .Connection.RemoteIpAddress!.ToString() ?? "UnKnown",
                cancellationToken: cancellationToken
            );

            return "تم تأكيد البريد الإلكتروني بنجاح";
        }
    }
}