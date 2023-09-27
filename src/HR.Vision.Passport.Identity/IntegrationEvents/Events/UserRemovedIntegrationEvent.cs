using System;

using Newtonsoft.Json;

namespace E.Shop.Passport.Identity.IntegrationEvents.Events
{
    public class UserRemovedIntegrationEvent
    {
        [JsonProperty("employee_id")]
        public Guid EmployeeId { get; set; }
    }
}