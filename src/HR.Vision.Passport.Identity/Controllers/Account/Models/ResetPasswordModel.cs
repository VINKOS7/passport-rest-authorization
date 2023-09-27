namespace E.Shop.Passport.Identity.Controllers.Account.Models
{
    public class ActivationModel
    {
        public string Email { get; set; }

        public string PasswordToken { get; set; }

        public string NewPassword { get; set; }
    }
}
