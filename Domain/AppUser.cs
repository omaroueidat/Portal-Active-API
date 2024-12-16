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

        // Activites that the user is attending
        public ICollection<ActivityAttendee> Activities { get; set; }

        // User have many images
        public ICollection<Photo> Photos { get; set; }

        // Collection for the Followings that the user follow
        public ICollection<UserFollowing> Followings { get; set; }

        // Collection for the followers that follow the user
        public ICollection<UserFollowing> Followers { get; set; }
    }
}
