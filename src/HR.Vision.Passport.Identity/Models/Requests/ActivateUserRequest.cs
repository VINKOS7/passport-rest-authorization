using Newtonsoft.Json;

namespace E.Shop.Passport.Identity.Models.Requests
{
    public class ActivateUserRequest
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}