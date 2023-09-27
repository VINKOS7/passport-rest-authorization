using Newtonsoft.Json;

namespace E.Shop.Passport.Identity.Services.Models
{
    public class ActivationEmailContent
    {
        [JsonProperty("activation_url")]
        public string ActivationUrl { get; set; }
    }
}