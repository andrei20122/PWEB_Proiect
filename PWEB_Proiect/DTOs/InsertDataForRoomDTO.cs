namespace PWEB_Proiect.DTOs
{
    public class InsertDataForRoomDTO
    {
        public string Username { get; set; } = default!;
        public double Medie { get; set; }
        public int An_studiu { get; set; }
        public string Programa { get; set; } = default!;
        public bool SpecialRights { get; set; }
    }
}
