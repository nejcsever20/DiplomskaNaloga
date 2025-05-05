using System.Net;
using System.Net.Mail;
using System.Text;

namespace diplomska.Services
{
    public class CustomEmailSender
    {
        private readonly string _smtpServer;
        private readonly string _fromEmail;

        public CustomEmailSender(IConfiguration configuration)
        {
            _smtpServer = configuration["Smtp:Server"];
            _fromEmail = configuration["Smtp:From"];
        }

        public async Task<bool> SendCustomHtmlEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using var mailMessage = new MailMessage(_fromEmail, toEmail)
                {
                    Subject = subject,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };

                var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                using var smtpClient = new SmtpClient(_smtpServer);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
                return false;
            }
        }
    }
}
