using System;

namespace E.Shop.Passport.Identity.Models
{
    public class UserActivation
    {
        public static UserActivation Create(Guid userId)
        {
            return new UserActivation
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };
        }
        
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }
    }
}