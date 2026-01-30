using MediatR;
using System.Text;
using HomeTech.Domain.Enums;
using HomeTech.Domain.Entities;
using HomeTech.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using HomeTech.Application.Interfaces;
using HomeTech.Application.Exceptions;
using HomeTech.Application.DTOs.Emails;
using Microsoft.AspNetCore.WebUtilities;
using HomeTech.Application.CQRS.Commands;
using Microsoft.Extensions.Configuration;

public class ChangeEmailCommandHandler
    : IRequestHandler<ChangeEmailCommand, string>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailQueue _emailQueue;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _http;

    public ChangeEmailCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailQueue emailQueue,
        IConfiguration config,
        IHttpContextAccessor http)
    {
        _userManager = userManager;
        _emailQueue = emailQueue;
        _config = config;
        _http = http;
    }

    public async Task<string> Handle(
        ChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new UserNotFoundException("هذا المستخدم غير موجود");

        user.PendingEmail = request.NewEmail;
        await _userManager.UpdateAsync(user);

        var token = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(
                await _userManager.GenerateEmailConfirmationTokenAsync(user)))
            ?? throw new TokenNotFoundException("جلسة غير صالحة او منتهية");

        var link =
            $"{_config["App:FrontendUrl"]}/confirm-email" +
            $"?email={user.Email}&token={token}";

        await _emailQueue.EnqueueEmailAsync(new EmailJob
        {
            To = request.NewEmail,
            Subject = "تأكيد تغيير البريد الإلكتروني",
            Template = EmailTemplate.ConfirmEmail,
            Model = new ConfirmEmailModel
            {
                FullName = user.FullName,
                ConfirmLink = link
            }
        });

        return "تم إرسال رابط تأكيد إلى البريد الجديد";
    }
}