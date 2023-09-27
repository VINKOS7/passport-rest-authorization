using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Dotseed.EventBus;

using E.Shop.Passport.Identity.Data;
using E.Shop.Passport.Identity.IntegrationEvents.Events;
using E.Shop.Passport.Identity.Models;
using E.Shop.Passport.Identity.Services;

namespace E.Shop.Passport.Identity.IntegrationEvents.EventHandlers
{
    public class UserAddedIntegrationEventHandler : IConsumer<UserAddedIntegrationEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivationService _activationService;
        private readonly ApplicationDbContext _db;

        public UserAddedIntegrationEventHandler(
            UserManager<ApplicationUser> userManager,
            IActivationService activationService,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _activationService = activationService;
            _db = db;
        }
        
        public async Task Consume(UserAddedIntegrationEvent message)
        {
            var user = await _userManager.FindByIdAsync(message.EmployeeId.ToString());
            if (user != null)
            {
                return;
            }
            
            user = new ApplicationUser
            {
                Id = message.EmployeeId.ToString(),
                Email = message.Email,
                PhoneNumber = message.Phone,
                UserName = message.Email,
                Surname = message.Surname,
                Name = message.Name
            };
            
            var result = await _userManager.CreateAsync(user);
            
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            
            result = await _userManager.AddClaimsAsync(user, new []
            {
                new Claim(JwtClaimTypes.Id, message.EmployeeId.ToString()), 
                new Claim(JwtClaimTypes.Name, $"{message.Name} {message.Surname}"),
                new Claim(JwtClaimTypes.GivenName, message.Name),
                new Claim(JwtClaimTypes.FamilyName, message.Surname),
                new Claim(JwtClaimTypes.Email, message.Email)
            });
            
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            var activation = UserActivation.Create(message.EmployeeId);
            _db.Activations.Add(activation);

            _db.Users.Add(user);
            
            await _activationService.SendActivationLink(
                user, 
                await _userManager.GeneratePasswordResetTokenAsync(user));
            
            await _db.SaveChangesAsync();
        }
    }
}