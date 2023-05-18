
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;

using Models.Settings;
using Models.Dtos.Email;

namespace Services.File
{
    public class MockEmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public MockEmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendAsync(EmailRequest request)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.EmailFrom);
            email.To.Add(MailboxAddress.Parse(_mailSettings.EmailTo));
            email.Subject = request.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = request.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.EmailFrom, _mailSettings.SmtpPass);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            _logger.LogInformation("has been sent");

        }
    }
}