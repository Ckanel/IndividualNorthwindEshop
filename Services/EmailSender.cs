using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
namespace IndividualNorthwindEshop.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // This is a dummy email sender that does nothing.
            
            return Task.CompletedTask;
        }
    }
}
