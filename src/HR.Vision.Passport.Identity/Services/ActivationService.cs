using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using E.Shop.Passport.Identity.Models;
using E.Shop.Passport.Identity.Options;
using E.Shop.Passport.Identity.Services.Models;

namespace E.Shop.Passport.Identity.Services
{
    public class ActivationService : IActivationService
    {
        private readonly IEmailService _emailService;
        private readonly string _activationHost;
        private const string ActivationEmailTemplateId = "d-ccd44390b63d4cc5b5ddc1e4e3c39514";
        private readonly string _defaultHost;

        public ActivationService(
            IEmailService emailService, 
            IOptions<UserActivationSettings> options,
            IOptions<DefaultHostSettings> hostOptions)
        {
            _emailService = emailService;
            _activationHost = options.Value.ActivationHost;
            _defaultHost = hostOptions.Value.MainFrontEndHost;           
        }
        
        public async Task<bool> SendActivationLink(ApplicationUser user, string code)
        {
            var userEmail = new Email
            {
                Address = user.Email,
                Name = $"{user.Surname} {user.Name}"
            };
            
            var mailContent = new EmailContent
            {
                TemplateId = ActivationEmailTemplateId,
                TemplateData = new ActivationEmailContent
                {
                    ActivationUrl = $"{_defaultHost}/Account/ActivationEnd?email={user.Email}?code={code}"
                }
            };

            return await _emailService.Send(userEmail, mailContent);
        }
    }
}