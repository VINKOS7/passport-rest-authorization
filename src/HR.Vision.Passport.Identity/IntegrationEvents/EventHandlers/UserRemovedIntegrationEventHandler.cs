using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Dotseed.EventBus;

using E.Shop.Passport.Identity.Data;
using E.Shop.Passport.Identity.IntegrationEvents.Events;
using E.Shop.Passport.Identity.Models;

namespace E.Shop.Passport.Identity.IntegrationEvents.EventHandlers
{
    public class UserRemovedIntegrationEventHandler : IConsumer<UserRemovedIntegrationEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public UserRemovedIntegrationEventHandler(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        
        public async Task Consume(UserRemovedIntegrationEvent message)
        {
            var user = await _userManager.FindByIdAsync(message.EmployeeId.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            var activation = await _db.Activations.FirstOrDefaultAsync(a => a.UserId == message.EmployeeId);
            if (activation != null)
            {
                _db.Activations.Remove(activation);
                await _db.SaveChangesAsync();
            }
        }
    }
}