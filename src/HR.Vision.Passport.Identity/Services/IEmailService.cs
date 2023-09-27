using System.Threading.Tasks;

using E.Shop.Passport.Identity.Services.Models;

namespace E.Shop.Passport.Identity.Services
{
    public interface IEmailService
    {
        Task<bool> Send(Email to, EmailContent content);
    }
}