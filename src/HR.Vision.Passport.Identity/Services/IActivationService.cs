using System.Threading.Tasks;
using E.Shop.Passport.Identity.Models;

namespace E.Shop.Passport.Identity.Services
{
    public interface IActivationService
    {
        Task<bool> SendActivationLink(ApplicationUser user, string code);
    }
}