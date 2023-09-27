using System;
using Newtonsoft.Json;

namespace E.Shop.Passport.Identity.IntegrationEvents.Events
{
    public class UserAddedIntegrationEvent
    {
        [JsonProperty("employee_id")]
        public Guid EmployeeId { get; set; }
        
        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("organization_id")]
        public Guid OrganizationId { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }
    }
}