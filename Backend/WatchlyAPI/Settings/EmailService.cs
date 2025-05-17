using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WatchlyAPI.Settings;
namespace WatchlyAPI.Settings
{
    public class EmailSettings
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            var emailAddress = _emailSettings.Email;
            var emailPassword = _emailSettings.Password;
            email.From.Add(MailboxAddress.Parse(emailAddress));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailAddress, emailPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public static string IncryptEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return email;
            }
            string newEmail = String.Empty;
            for (int i = 0; i < email.Length; i++)
            {
                if (email[i] == '@')
                {
                    newEmail = email.Substring(0, i) + new string('*', email.Length - i);
                }


            }
            return newEmail;
        }
    }
}
