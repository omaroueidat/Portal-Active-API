using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        
        // AppUser which is the author of the comment
        public AppUser Author { get; set; }

        // Activity that this comment belongs to
        public Activity Activity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
