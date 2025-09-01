using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace AiMarketing.Blazor.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _config["Email:SendGridApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("admin@prompt-n-post.com", "Prompt-N-Post");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: htmlMessage);
            await client.SendEmailAsync(msg);
        }
    }
}
