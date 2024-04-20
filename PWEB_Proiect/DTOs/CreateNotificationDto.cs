namespace PWEB_Proiect.DTOs
{
    public class CreateNotificationDto
    {
        public string Username { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
    }
}
