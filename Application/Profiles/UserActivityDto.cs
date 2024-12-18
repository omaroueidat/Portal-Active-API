using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Profiles
{
    public class UserActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }

        // Helper Attribute
        [JsonIgnore]
        public string HostUsername { get; set; }
    }
}
