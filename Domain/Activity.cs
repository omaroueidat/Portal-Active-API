
namespace Domain
{
    public class Activity
    {
        public Guid Id { get; set; } 
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public bool IsCancelled { get; set; }

        // Relation: Activity and AppUser Many to Many => Activity (1,n) --- ActivityAttendee -- (0,n) AppUser
        public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>();

        // Relation: Activity have many Comments => Activty (1,1) ---> (0,n) Comments 
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
