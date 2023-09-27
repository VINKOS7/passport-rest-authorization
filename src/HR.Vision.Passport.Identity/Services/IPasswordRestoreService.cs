using System.Threading.Tasks;
using E.Shop.Passport.Identity.Models;

namespace E.Shop.Passport.Identity.Services
{
    public interface IPasswordRestoreService
    {
        Task<bool> SendRestoreLink(ApplicationUser user, string token);
    }
}