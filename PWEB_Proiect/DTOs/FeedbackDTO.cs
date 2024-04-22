namespace PWEB_Proiect.DTOs
{
    public class FeedbackDTO
    {
        public string? Username { get; set; }
        public string Subject { get; set; }
        public bool IsSatisfied { get; set; }
        public List<string> Features { get; set; }
        public string Comments { get; set; }
    }
}
