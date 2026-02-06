using MediatR;
using System.Text;
using HomeTech.Domain.Enums;
using HomeTech.Domain.Entities;
using HomeTech.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Exceptions;
using HomeTech.Application.Interfaces;
using HomeTech.Application.DTOs.Emails;
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
        private readonly IAuditService _audit;
        private readonly IHttpContextAccessor _http;
        private readonly IFileStorageService _fileStorage;

        public RegisterUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmailQueue emailQueue,
            IConfiguration config,
            IAuditService audit,
            IHttpContextAccessor http,
            IFileStorageService fileStorage)
        {
            _userManager = userManager;
            _emailQueue = emailQueue;
            _config = config;
            _audit = audit;
            _http = http;
            _fileStorage = fileStorage;
        }

        public async Task<string> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Dto.Email.Trim()) != null)
                throw new FailedRegisterUserException("البريد الإلكتروني مستخدم بالفعل");

            var imagePath = await _fileStorage.SaveAsync(request.Dto.formFile, "users");

            var user = new ApplicationUser
            {
                FullName = request.Dto.FullName.Trim(),
                UserName = request.Dto.Email.Trim(),
                Email = request.Dto.Email.Trim(),
                Gender = request.Dto.Gender,
                ProfileImagePath = imagePath
            };

            var result = await _userManager.CreateAsync(
                user, request.Dto.Password.Trim()
            );

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                await _audit.LogActionAsync(
                    userId: null,
                    sessionId: null,
                    action: "Failed User Registration",
                    reason: errors,
                    ipAddress: _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    cancellationToken: cancellationToken
                );
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
                Template = EmailTemplate.ConfirmEmail,
                Model = new ConfirmEmailModel
                {
                    FullName = user.FullName.Trim(),
                    ConfirmLink = confirmLink.Trim()
                }
            });

            var ip = _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            await _audit.LogActionAsync(
                userId: user.Id,
                sessionId: null,
                action: "User Registration",
                reason: "New user registered",
                ipAddress: ip,
                cancellationToken: cancellationToken
            );

            return "تم إنشاء الحساب بنجاح 🎉 برجاء تأكيد بريدك الإلكتروني";
        }
    }
}