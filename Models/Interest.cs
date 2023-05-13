namespace Labb_3.Models
{
    public class Interest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<InterestLink> Links { get; set; } = new List<InterestLink>();
        public ICollection<PersonInterest> PersonInterests { get; set; } = new List<PersonInterest>();
    }
}
