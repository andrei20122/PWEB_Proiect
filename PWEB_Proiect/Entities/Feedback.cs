namespace PWEB_Proiect.Entities
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Subject { get; set; }
        public bool IsSatisfied { get; set; }
        public List<string>? Features { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedTime { get; set; }
        public User User { get; set; }

    }
}
