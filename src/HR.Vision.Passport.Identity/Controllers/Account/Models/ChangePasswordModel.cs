using System;

namespace E.Shop.Passport.Identity.Controllers.Account.Models
{
    public enum ErrorChangePassword
    {
        errorPassword,
        errorEmail
    }

    public class ChangePasswordModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public ErrorChangePassword Error { get; set; }

        public static ChangePasswordModel GetThisWithError(ErrorChangePassword value)
        {
            return new ChangePasswordModel
            {
                Email = string.Empty,
                Error = value,
                Password = string.Empty,
                NewPassword = string.Empty,
            };
        }
    }
}
