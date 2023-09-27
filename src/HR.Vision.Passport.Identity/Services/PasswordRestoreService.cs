using System.Threading.Tasks;
using E.Shop.Passport.Identity.Models;
using E.Shop.Passport.Identity.Options;
using E.Shop.Passport.Identity.Services.Models;
using Microsoft.Extensions.Options;

namespace E.Shop.Passport.Identity.Services
{
    public class PasswordRestoreService : IPasswordRestoreService
    {
        private readonly IEmailService _emailService;
        private readonly string _activationHost;
        private const string RestorePasswordTemplateId = "d-55b5882f22b9474286f4bd3fd30645cb";

        public PasswordRestoreService(IEmailService emailService, IOptions<UserActivationSettings> options)
        {
            _emailService = emailService;
            _activationHost = options.Value.ActivationHost;
        }
        
        public async Task<bool> SendRestoreLink(ApplicationUser user, string token)
        {
            var userEmail = new Email
            {
                Address = user.Email,
                Name = $"{user.Surname} {user.Name}"
            };
            
            var mailContent = new EmailContent
            {
                TemplateId = RestorePasswordTemplateId,
                TemplateData = new RestorePasswordEmailContent
                {
                    RestoreUrl = $"{_activationHost}/password/restore?token={token}&userId={user.Id}"
                }
            };

            return await _emailService.Send(userEmail, mailContent);
        }
    }
}