using Newtonsoft.Json;

namespace E.Shop.Passport.Identity.Services.Models
{
    public class RestorePasswordEmailContent
    {
        [JsonProperty("restore_url")]
        public string RestoreUrl { get; set; }
    }
}