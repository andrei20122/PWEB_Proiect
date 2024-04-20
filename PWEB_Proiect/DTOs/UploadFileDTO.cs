namespace PWEB_Proiect.DTOs
{
    public class UploadFileDTO
    {
        public IEnumerable< IFormFile> Files { get; set; } = default!;
        public string Username { get; set; } = default!;

    }
}
