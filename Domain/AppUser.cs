using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }

        // For Reference
        public ICollection<ActivityAttendee> Activities { get; set; }

        // User have many images
        public ICollection<Photo> Photos { get; set; }
    }
}
