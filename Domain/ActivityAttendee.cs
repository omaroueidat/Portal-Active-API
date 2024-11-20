using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ActivityAttendee
    {
        // Foriegn Key references AppUser.ID
        public string AppUserId { get; set; }

        // Forign Key references Activity.Id
        public Guid ActivityId { get; set; }

        // Current User is the host or not
        public bool IsHost { get; set; }

        // Reference Attribues
        public AppUser AppUser { get; set; }
        public Activity Activity { get; set; }
    }
}
