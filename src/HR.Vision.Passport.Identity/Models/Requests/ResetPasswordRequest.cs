using System;
using Newtonsoft.Json;

namespace E.Shop.Passport.Identity.Models.Requests
{
    public class ResetPasswordRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}