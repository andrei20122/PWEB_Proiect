namespace PWEB_Proiect.DTOs
{
    public class UpdateDocumentStatusRequestDTO
    {
        public string Status { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}
