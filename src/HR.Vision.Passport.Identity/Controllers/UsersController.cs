using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using E.Shop.Passport.Identity.Data;
using E.Shop.Passport.Identity.Models;
using E.Shop.Passport.Identity.Models.Responses;
using E.Shop.Passport.Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace E.Shop.Passport.Identity.Controllers
{
    [ApiController]
    [Route("users")]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        
        public UsersController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost]
        public async Task Create([FromBody] AddUserRequest request)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                Name = request.Name,
                Id = request.Id.ToString(),
                Surname = request.Surname,
                UserName = request.Email
            };
            var result = await _userManager.CreateAsync(user);
            await _userManager.AddPasswordAsync(user, "123");
        }

        [HttpGet("activation/{id:Guid}")]
        public async Task<UserActivationData> GetActivation(Guid id)
        {
            var activation = await _db.Activations.FirstOrDefaultAsync(a => a.Id == id);
            if (activation == null)
            {
                return null;
            }
            
            var user = await _userManager.FindByIdAsync(activation.UserId.ToString());
            return UserActivationData.FromDomain(user);
        }

        [HttpPost("activation/{id:Guid}")]
        public async Task ActivateUser(Guid id, [FromBody] ActivateUserRequest request)
        {
            var activation = await _db.Activations.FirstOrDefaultAsync(a => a.Id == id);
            var user = await _userManager.FindByIdAsync(activation.UserId.ToString());

            var result = await _userManager.AddPasswordAsync(user, request.Password);
            if (result.Succeeded)
            {
                _db.Activations.Remove(activation);
                await _db.SaveChangesAsync();
            }
        }

        [HttpPost("{id:Guid}/password/reset")]
        public async Task ResetPassword(Guid id, [FromBody] ResetPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
        }

        [HttpDelete("{id:Guid}")]
        public async Task RemoveUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        [HttpDelete]
        public async Task RemoveUser([FromQuery] string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        [Route("AdminRequirement")]
        [HttpPost("activation/{id:Guid}/role/add")]
        public async Task AddRoleUser(Guid id, AddUserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                await _userManager.AddClaimsAsync(user, request.Claims);
            }
        }
    }
}