using AiMarketing.Blazor.Components.Account.Pages.Manage;
using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;

namespace AiMarketing.Blazor.Services.EmailSender
{
    public class IdentityEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IEmailService _emailService;

        public IdentityEmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            var subject = "Confirm your email";
            var html = $@"
                        <p>Welcome to <strong>Prompt-N-Post</strong>!</p>
                        <p>Click below to confirm your account:</p>
                        <p><a href=""{confirmationLink}""
                              style=""display:inline-block;padding:12px 18px;border-radius:8px;background:#111;color:#fff;
                                     text-decoration:none;font-weight:600"">Confirm my email</a></p>
                        <p style=""font-size:12px;color:#666"">If the button doesn’t work, copy and paste this URL:<br/>{HtmlEncoder.Default.Encode(confirmationLink)}</p>";
            return _emailService.SendEmailAsync(email, subject, html);
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            var subject = "Your password reset code";
            var html = $@"<p>Your code: <strong>{resetCode}</strong></p>";
            return _emailService.SendEmailAsync(email, subject, html);
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            var subject = "Reset your password";
            var html = $@"
                        <p>You requested a password reset for <strong>Prompt-N-Post</strong>.</p>
                        <p><a href=""{resetLink}""
                              style=""display:inline-block;padding:12px 18px;border-radius:8px;background:#111;color:#fff;
                                     text-decoration:none;font-weight:600"">Reset password</a></p>
                        <p style=""font-size:12px;color:#666"">If the button doesn’t work, copy and paste this URL:<br/>{HtmlEncoder.Default.Encode(resetLink)}</p>";
            return _emailService.SendEmailAsync(email, subject, html);

        }
    }
}
