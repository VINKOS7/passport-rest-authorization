using System.Net;
using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;

using E.Shop.Passport.Identity.Services.Models;

namespace E.Shop.Passport.Identity.Services
{
    public class EmailService : IEmailService
    {
        private const string ApiKey = "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";
        private readonly Email _robot = new Email
        {
            Address = "robot@hr.vision",
            Name = "HR Vision"
        };

        public async Task<bool> Send(Email to, EmailContent content)
        {
            var client = new SendGridClient(ApiKey);
            var emailFrom = new EmailAddress(_robot.Address, _robot.Name);
            var emailTo = new EmailAddress(to.Address, to.Name);
            var message = MailHelper.CreateSingleTemplateEmail(
                emailFrom,
                emailTo,
                content.TemplateId,
                content.TemplateData);

            var result = await client.SendEmailAsync(message);
            return result.StatusCode == HttpStatusCode.Accepted;
        }

        public async Task<bool> Send(Email to, EmailData content)
        {
            var client = new SendGridClient(ApiKey);
            var emailFrom = new EmailAddress(_robot.Address, _robot.Name);
            var emailTo = new EmailAddress(to.Address, to.Name);
            var message = MailHelper.CreateSingleEmail(
                           emailFrom,
                           emailTo,
                           content.Subject,
                           content.Data,
                           content.HtmlContent);

            var result = await client.SendEmailAsync(message);
            return result.StatusCode == HttpStatusCode.Accepted;
        }
    }
}