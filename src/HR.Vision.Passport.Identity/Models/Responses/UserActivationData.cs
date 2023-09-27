using System;

using Newtonsoft.Json;

namespace E.Shop.Passport.Identity.Models.Responses
{
    public class UserActivationData
    {
        public static UserActivationData FromDomain(ApplicationUser user)
        {
            return new UserActivationData
            {
                Id = Guid.Parse(user.Id),
                Surname = user.Surname,
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumber
            };
        }
        
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}