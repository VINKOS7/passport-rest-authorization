using System;

using Newtonsoft.Json;

namespace E.Shop.Passport.Identity.Models.Requests
{
    public class AddUserRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        
        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}