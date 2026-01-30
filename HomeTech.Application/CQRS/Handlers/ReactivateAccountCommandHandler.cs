using MediatR;
using System.Text;
using HomeTech.Domain.Enums;
using HomeTech.Domain.Entities;
using HomeTech.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Interfaces;
using HomeTech.Application.Exceptions;
using HomeTech.Application.DTOs.Emails;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class ReactivateAccountCommandHandler
        : IRequestHandler<ReactivateAccountCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailQueue _emailQueue;
        private readonly IConfiguration _config;

        public ReactivateAccountCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmailQueue emailQueue,
            IConfiguration config)
        {
            _userManager = userManager;
            _emailQueue = emailQueue;
            _config = config;
        }

        public async Task<string> Handle(
            ReactivateAccountCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new UserNotFoundException("هذا المستخدم غير موجود");

            if (user.IsActive) return "الحساب مفعل بالفعل";

            var token = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(
                    await _userManager.GenerateEmailConfirmationTokenAsync(user)))
                ?? throw new TokenNotFoundException("جلسة غير صالحة او منتهية");

            var link =
                $"{_config["App:FrontendUrl"]}/confirm-email" +
                $"?email={user.Email}&token={token}";

            await _emailQueue.EnqueueEmailAsync(new EmailJob
            {
                To = user.Email!,
                Subject = "إعادة تفعيل الحساب",
                Template = EmailTemplate.ConfirmEmail,
                Model = new ConfirmEmailModel
                {
                    FullName = user.FullName,
                    ConfirmLink = link
                }
            });

            return "تم إرسال رابط إعادة التفعيل";
        }
    }
}