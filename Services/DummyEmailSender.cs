using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace diplomska.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Email sen to {email}: {subject}");
            return Task.CompletedTask;
        }
    }
}
