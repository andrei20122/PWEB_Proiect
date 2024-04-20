namespace PWEB_Proiect.DTOs
{
    public class UpdateProfile
    {
        public string Old_username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Cnp { get; set; }
        public string Sex { get; set; } = string.Empty;
    }
}
