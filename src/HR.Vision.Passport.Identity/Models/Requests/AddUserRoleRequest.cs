using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace E.Shop.Passport.Identity.Models.Requests
{
    public record AddUserRoleRequest
    {
        [JsonProperty("Claims")]
        public IEnumerable<Claim> Claims { get; set; }
    }
}
