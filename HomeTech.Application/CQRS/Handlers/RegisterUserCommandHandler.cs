using MediatR;
using System.Text;
using HomeTech.Domain.Entities;
using HomeTech.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Exceptions;
using HomeTech.Application.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.CQRS.Handlers
{
    public class RegisterUserCommandHandler :
        IRequestHandler<RegisterUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailQueue _emailQueue;
        private readonly IConfiguration _config;

        public RegisterUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmailQueue emailQueue,
            IConfiguration config)
        {
            _userManager = userManager;
            _emailQueue = emailQueue;
            _config = config;
        }

        public async Task<string> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Dto.Email) != null)
                throw new FailedRegisterUserException("البريد الإلكتروني مستخدم بالفعل");

            var user = new ApplicationUser
            {
                FullName = request.Dto.FullName,
                UserName = request.Dto.Email,
                Email = request.Dto.Email
            };

            var result = await _userManager.CreateAsync(
                user, request.Dto.Password
            );

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new FailedRegisterUserException(errors);
            }

            var token = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(
                    await _userManager.GenerateEmailConfirmationTokenAsync(user)
                )
            );

            var confirmLink =
                $"{_config["App:FrontendUrl"]}/confirm-email" +
                $"?email={user.Email}&token={token}";

            await _emailQueue.EnqueueEmailAsync(new EmailJob
            {
                To = user.Email!,
                Subject = "تأكيد البريد الإلكتروني",
                Body = $"اضغط هنا لتأكيد حسابك 👇<br/><a href='{confirmLink}'>تأكيد البريد</a>"
            });

            return "تم إنشاء الحساب بنجاح 🎉 برجاء تأكيد بريدك الإلكتروني";
        }
    }
}