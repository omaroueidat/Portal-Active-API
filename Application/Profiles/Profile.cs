using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Profiles
{
    public class Profile
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }

        // Bool to see if the current user that is getting the profile follows this user or not
        public bool Following { get; set; }

        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }

        // Photos of the user
        public ICollection<Photo> Photos { get; set; }


    }
}
