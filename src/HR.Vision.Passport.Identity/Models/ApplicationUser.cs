using System;
using Microsoft.AspNetCore.Identity;

namespace E.Shop.Passport.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Surname { get; set; }

        public string Name { get; set; }
    }
}