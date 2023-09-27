using System.ComponentModel.DataAnnotations;

namespace E.Shop.Passport.Identity.Controllers.Account
{
    public class RestorePasswordInputModel
    {
        [Required]
        public string Username { get; set; }
    }
}