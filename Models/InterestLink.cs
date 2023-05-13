namespace Labb_3.Models
{
    public class InterestLink
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public int InterestId { get; set; }
        public Interest Interest { get; set; }
    }
}
