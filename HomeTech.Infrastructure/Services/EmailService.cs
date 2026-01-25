using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using HomeTech.Application.Exceptions;
using HomeTech.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HomeTech.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new SMTPException("Recipient email is required");

            var smtpEmail = _configuration["Smtp:Email"]
                ?? throw new SMTPException("SMTP Email missing");

            var smtpPassword = _configuration["Smtp:Password"]
                ?? throw new SMTPException("SMTP Password missing");

            var host = _configuration["Smtp:Host"]
                ?? throw new SMTPException("SMTP Host missing");

            var port = int.Parse(_configuration["Smtp:Port"] ?? "587");

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(smtpEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = body
            }.ToMessageBody();

            try
            {
                using var client = new SmtpClient();

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpEmail, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (SmtpCommandException ex)
            {
                throw new SMTPException($"SMTP Command Error: {ex.Message}");
            }
            catch (SmtpProtocolException ex)
            {
                throw new SMTPException($"SMTP Protocol Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new SMTPException($"Email Service Error: {ex.Message}");
            }
        }
    }
}