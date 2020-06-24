using System;
using Newtonsoft.Json;

namespace Assessment.Domain
{
    public class Employee
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("employmentStartDate")]
        public DateTime EmploymentStartDate { get; set; }

        [JsonProperty("employmentEndDate")]
        public DateTime? EmploymentEndDate { get; set; }
    }
}
