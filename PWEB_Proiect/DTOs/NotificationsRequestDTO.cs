namespace PWEB_Proiect.DTOs
{
    public class NotificationsRequestDTO
    {
        public string Username { get; set; } = default!;
        public int[] Types { get; set; }
    }
}
