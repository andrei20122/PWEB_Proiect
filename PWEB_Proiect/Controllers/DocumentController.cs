using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PWEB_Proiect.DTOs;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PWEB_Proiect.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DocumentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private bool AllowedFile(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".pdf", ".docx" };
            return Array.IndexOf(allowedExtensions, fileExtension) != -1;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm] UploadFileDTO fileDTO)
        {
            List<string> failedFiles = new List<string>();

            if (fileDTO == null)
                return Ok(new ErrorMessageDTO { Error = "No fileDTO part in the request" });

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == fileDTO.Username);
            if (user == null)
                return Ok(new ErrorMessageDTO { Error = "User not found" });

            string uploadFolder = Path.Combine(_environment.ContentRootPath, "UploadedFiles");
            Directory.CreateDirectory(uploadFolder);

            foreach (IFormFile file in fileDTO.Files)
            {
                if (!AllowedFile(file.FileName))
                {
                    failedFiles.Add(file.FileName);
                    continue;
                }

                string fileName = Path.GetFileName(file.FileName);
                string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

                string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff");
                string sanitizedFileName = string.Concat(Path.GetFileNameWithoutExtension(fileName).Take(150));
                sanitizedFileName = sanitizedFileName + fileExtension;
                string documentId = $"{fileDTO.Username}_{timestamp}_{sanitizedFileName}";
                string filePath = Path.Combine(uploadFolder, documentId);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var document = new Documents
                    {
                        Id = documentId,
                        UploadTime = DateTime.UtcNow,
                        Type = fileExtension,
                        UserId = user.Id,
                        Status = "Pending",
                        Feedback = ""
                    };

                    _context.Documents.Add(document);
                }
                catch (Exception ex)
                {
                    failedFiles.Add(file.FileName);
                    // Optionally log the exception here
                }
            }

            await _context.SaveChangesAsync();

            if (failedFiles.Count > 0)
            {
                return Ok(new
                {
                    message = "Some files were not uploaded due to invalid type",
                    uploadedFiles = fileDTO.Files.Count() - failedFiles.Count,
                    failedFiles = failedFiles
                });
            }

            return Ok(new { message = "All files successfully uploaded" });
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UploadPhoto([FromForm] UploadPhotoDTO file)
        {
            string? username= User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name, null)?.Value;
            if (file == null)
                return Ok(new ErrorMessageDTO() { Error = "No fileDTO selected for uploading" });

            string extension = Path.GetExtension(file.Photo.FileName).ToLowerInvariant();
            if(!(extension == ".jpg" || extension == ".jpeg" || extension == ".png"))
                return Ok(new ErrorMessageDTO() { Error = "File type is not allowed" });

            string fileName = Path.GetFileName(file.Photo.FileName);
            string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return Ok(new ErrorMessageDTO() { Error = "User not found" });

            string uploadFolder = Path.Combine(_environment.ContentRootPath, "UploadedPhotos");
            Directory.CreateDirectory(uploadFolder);

            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff"); // Format compatibil cu sistemele de fișiere
            //data cand s-a uploadat fisierul, ora , minutul, secunda si milisecunda
            string sanitizedFileName = string.Concat(Path.GetFileNameWithoutExtension(fileName).Take(150)); // Limitează lungimea și elimină caracterele interzise
            sanitizedFileName += fileExtension; // Adaugă timestamp și extensie
            string student_photo = $"{username}_{user.Id}_{timestamp}_{sanitizedFileName}";
            string filePath = Path.Combine(uploadFolder, student_photo);

            try
            {
                var fileStream = System.IO.File.Open(filePath, FileMode.CreateNew );
                file.Photo.CopyTo(fileStream);
                fileStream.Close();

                // Delete the old photo if it exists
                if (!string.IsNullOrEmpty(user.StudentPhoto) && System.IO.File.Exists(Path.Combine(uploadFolder, user.StudentPhoto)))
                {
                    System.IO.File.Delete(Path.Combine(uploadFolder, user.StudentPhoto));
                }

                // Update user's photo
                user.StudentPhoto = student_photo;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessageDTO() { Error = ex.Message });
            }

            return Ok(new { message = "Photo successfully uploaded", student_photo });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetDocuments()
        {
            var username = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name, null)?.Value;
            if (username == null)
                return Ok(new ErrorMessageDTO() { Error = "No username available" });

            try
            {
                var documents = await _context.Documents
                    .Where(d => (d.Type == ".docx" || d.Type == ".pdf") && d.Id.StartsWith(username + "_"))
                    .Select(d => new
                    {
                        d.Id,
                        d.Type,
                        d.UploadTime, // Presupunând că UploadTime este un DateTime
                        d.Status,
                        d.Feedback,
                        Url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/UploadedFiles/{d.Id}"
                    })
                    .ToListAsync();

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessageDTO() { Error = ex.Message });
            }
        }

        
        [Authorize(Roles = "admin")] // am pus rolul de admin pentru a putea face update la statusul documentului, dar cu a mic...
        [HttpPost("{documentId}")]
        public async Task<IActionResult> UpdateDocumentStatus(string documentId, [FromBody] UpdateDocumentStatusRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.Status))
            {
                return Ok(new ErrorMessageDTO() { Error = "New status is required." });
            }

            var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
            if (document == null)
            {
                return Ok(new ErrorMessageDTO { Error = "Document not found." });
            }

            document.Status = request.Status;
            document.Feedback = request.Feedback ?? string.Empty;

            try
            {
                _context.Documents.Update(document);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Document status updated successfully." });
            }
            catch (DbUpdateException ex)
            {
                return Ok(new ErrorMessageDTO() { Error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{username}")] // !! practic le sterge pe toate nu pe alea cu DECLINED, dar trebuie sa ma mai gandesc la asta 
        public async Task<IActionResult> DeleteDeclinedDocuments(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return Ok(new ErrorMessageDTO() { Error = "User not found" });
            }

            var declinedDocs = await _context.Documents
                .Where(d =>d.UserId == user.Id)
                .ToListAsync();

            foreach (var doc in declinedDocs)
            {
                string docPath = Path.Combine(_environment.ContentRootPath, "UploadedFiles", doc.Id);
                if (System.IO.File.Exists(docPath))
                {
                    System.IO.File.Delete(docPath);
                }
                _context.Documents.Remove(doc);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Declined documents deleted successfully" });
        }

        

    }
}
