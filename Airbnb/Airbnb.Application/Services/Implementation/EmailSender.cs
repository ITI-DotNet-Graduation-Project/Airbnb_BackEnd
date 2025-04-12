using Airbnb.Application.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Airbnb.Application.Services.Implementation
{
    public class EmailSender : IEmailSender
    {
        private EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_emailSettings.Email),
                    Subject = subject
                };

                message.To.Add(MailboxAddress.Parse(email));

                var builder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };

                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);


            }
            catch (Exception ex)
            {
                // هنا تقدري تعملي Log للخطأ أو ترميه تاني
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
                throw; // تقدرِ تشيليه لو مش عايزة ترميه فوق
            }
        }
    }
}
