namespace E.Shop.Passport.Identity.Controllers.Account.Models
{
    public class ChangeForgotPasswordModel
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }



    }
}
