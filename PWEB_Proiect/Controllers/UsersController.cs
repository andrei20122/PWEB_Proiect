using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PWEB_Proiect.DTOs;
using PWEB_Proiect.Entities;

namespace PWEB_Proiect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UsersController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        [Authorize(Roles="admin")]
        [HttpGet("get_sorted_students")]
        public async Task<IActionResult> GetSortedStudentsMD()
        {
            var AllStudentsDisab = await _context.Users
                .Where(u => u.Role == "student" && u.SpecialRights == true)
                .Include(u => u.Preferences)
                .OrderByDescending(u => u.Programa) // Sortează după programă
                .ThenByDescending(u => u.An_studiu)
                .ThenByDescending(u => u.Medie)
                .ToListAsync();

            var AllStudentsNoDisab = await _context.Users
                .Where(u => u.Role == "student" && u.SpecialRights == false)
                .Include(u => u.Preferences)
                .OrderByDescending(u => u.Programa) // Sortează după programă
                .ThenByDescending(u => u.An_studiu)
                .ThenByDescending(u => u.Medie)
                .ToListAsync();

            var AllStudents = await _context.Users.Where(u => u.Role == "student")
                .Include(u => u.Preferences)
                .OrderByDescending(u => u.Programa) // Sortează după programă
                .ThenByDescending(u => u.An_studiu)
                .ThenByDescending(u => u.Medie)
                .ToListAsync();

            for(int i = 0; i < AllStudents.Count; i++)
                if (AllStudents[i].CurrentRoomId != null)
                    AllStudents[i].AssignedRoom = AllStudents[i].CurrentRoomId;

            List<Room> rooms = await _context.Rooms.ToListAsync();
            _context.ChangeTracker.AutoDetectChangesEnabled = true;

            List<User> StudentsNoPrefDisab = [];
            List<User> StudentsNoPref = [];

            for (int i = 0; i < AllStudentsDisab.Count; i++)
            {
                foreach (var pref in AllStudentsDisab[i].Preferences)
                {
                    int ok = 0;
                    string sex = "";
                    for (int k = 0; k < AllStudents.Count; k++)
                    {
                        if ((AllStudents[k].AssignedRoom == pref.RoomId))
                        {
                            ok++;
                            sex = AllStudents[k].Sex;
                        }

                    }

                    if (AllStudentsDisab[i].Sex == sex)
                        for (int g = 0; g < rooms.Count; g++)
                        {
                            if (rooms[g].Id == pref.RoomId)
                                if (rooms[g].Capacity < ok)
                                {
                                    AllStudentsDisab[i].AssignedRoom = pref.RoomId;
                                    break;
                                }
                        }

                    if (AllStudentsDisab[i].AssignedRoom != null)
                        break;
                }

                if (AllStudentsDisab[i].AssignedRoom == null)
                    StudentsNoPrefDisab.Add(AllStudentsDisab[i]);
            }

            for (int i = 0; i < AllStudentsNoDisab.Count; i++)
            {
                foreach (var pref in AllStudentsNoDisab[i].Preferences)
                {
                    int ok = 0;
                    string sex = "";
                    for (int k = 0; k < AllStudents.Count; k++)
                    {

                        if ((AllStudents[k].AssignedRoom == pref.RoomId))
                        {
                            ok++;
                            sex = AllStudents[k].Sex;
                        }

                    }

                    if (AllStudentsNoDisab[i].Sex == sex)
                        for (int g = 0; g < rooms.Count; g++)
                        {
                            if (rooms[g].Id == pref.RoomId)
                                if (rooms[g].Capacity < ok)
                                {
                                    AllStudentsNoDisab[i].AssignedRoom = pref.RoomId;
                                    break;
                                }
                        }

                    if (AllStudentsNoDisab[i].AssignedRoom != null)
                        break;
                }

                if (AllStudentsNoDisab[i].AssignedRoom == null)
                    StudentsNoPref.Add(AllStudentsNoDisab[i]);
            }

            var camereParter = rooms.Where(r => r.Floor == 0).ToList();
            if(camereParter.Count > 0)
                foreach (var plebeu in StudentsNoPrefDisab)
                    while (true)
                    {
                        int camera_rnd = new Random().Next(0, camereParter.Count);
                        int studenti_in_camera = AllStudents.Where(s => s.AssignedRoom == camereParter[camera_rnd].Id).Count();
                        string sexul_camerei = AllStudents.Where(s => s.AssignedRoom == camereParter[camera_rnd].Id).Select(s => s.Sex).First().ToString();

                        if (studenti_in_camera < camereParter[camera_rnd].Capacity && plebeu.Sex == sexul_camerei)
                        {
                            plebeu.AssignedRoom = camereParter[camera_rnd].Id;
                            break;
                        }
                    }
            

            foreach (var plebeu in StudentsNoPref)
                while (true)
                {
                    int camera_rnd = new Random().Next(0, rooms.Count);
                    int studenti_in_camera = AllStudents.Where(s => s.AssignedRoom == rooms[camera_rnd].Id).Count();

                    string? sexul_camerei = null;
                    if(studenti_in_camera > 0)
                        sexul_camerei = AllStudents.Where(s => s.AssignedRoom == rooms[camera_rnd].Id).Select(s => s.Sex).First().ToString();

                    if (studenti_in_camera < rooms[camera_rnd].Capacity && (sexul_camerei == null || plebeu.Sex == sexul_camerei))
                    {
                        plebeu.AssignedRoom = rooms[camera_rnd].Id;
                        break;
                    }
                }


            var changes = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified
                || e.State == EntityState.Added
                || e.State == EntityState.Deleted);

            if (!changes.Any())
            {
                return Ok(new ErrorMessageDTO() { Error = "No changes to save" });
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Rooms assigned successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Ok(new ErrorMessageDTO() { Error = "Internal server error" });
            }


        }

        [Authorize]
        [HttpPost("insert_data_for_room")]
        public async Task<IActionResult> InsertDataForRoom([FromBody] InsertDataForRoomDTO request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return Ok(new ErrorMessageDTO() { Error = "Invalid data" });
            }

            user.Medie = request.Medie;
            user.An_studiu = request.An_studiu;
            if(request.Programa == "Licenta")
            {
                user.Programa = 1;
            }
            else if(request.Programa == "Master")
            {
                user.Programa = 2;
            }
            else if(request.Programa == "Doctorat")
            {
                user.Programa = 3;
            }
            else
            {
                user.Programa = 0;
            }
            user.SpecialRights = request.SpecialRights;

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Data updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Ok(new ErrorMessageDTO() { Error = "Internal server error" });
            }
        }

        // GET: Users
        [Authorize]
        [HttpGet("get_user")]
        public async Task<IActionResult> GetUser()
        {
            var username = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name, null)?.Value;
            if (username == null)
                return Ok(new ErrorMessageDTO() { Error = "No username available" });

            try
            {
                var user = await _context.Users
                    .Where(u => u.Username == username)
                    .Select(u => new
                    {
                        u.Username,
                        u.Role,
                        u.Firstname,
                        u.Lastname,
                        u.Cnp,
                        u.Sex,
                        //deocamdata nu avem nevoie de toate proprietatile care tin de camera
                        u.CurrentRoomId,
                        u.AssignedRoom
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return Ok(new ErrorMessageDTO() { Error = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception e)
            {
                // Log the exception as well
                return Ok(new ErrorMessageDTO() { Error = "Internal server error" });
            }
        }


        public static byte[] GenerateSalt(int byteLength = 16)
        {
            var salt = new byte[byteLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        [HttpPost(Name = "CreateUser")]
        public bool CreateUser([FromBody] UserCreateRequestDTO userData)
        {
            if (_context.Users.Any(u => u.Username == userData.Username || u.Cnp == userData.Cnp))
            {
                return false;
            }

            var salt = GenerateSalt();

            string savedPasswordHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(userData.Password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                savedPasswordHash = Convert.ToBase64String(hashBytes);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = userData.Username,
                Password = savedPasswordHash,
                Role = userData.Role,
                CreatedAt = DateTime.UtcNow,
                Firstname = userData.FirstName,
                Lastname = userData.LastName,
                Cnp = userData.Cnp,
                Sex = userData.Sex,
                Salt = salt
            };

            
            _context.Users.Add(user);
            _context.SaveChanges();

           
            return true;
        }

        [Authorize]
        [HttpPost("get_student_photo")]
        public async Task<IActionResult> GetStudentPhoto(UsernameRequestDTO username)
        {
            var token = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name, null)?.Value;
            if (token == null)
                return Ok(new ErrorMessageDTO() { Error = "No token available" });

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username.Username);
            if (user?.StudentPhoto != null)
            {
                var webRoot = _environment.WebRootPath ?? _environment.ContentRootPath;
                var photoPath = Path.Combine(webRoot, "UploadedPhotos", user.StudentPhoto);

                if (System.IO.File.Exists(photoPath))
                {
                    var memoryStream = new MemoryStream();
                    await using (var stream = new FileStream(photoPath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memoryStream);
                    }
                    memoryStream.Position = 0; // Reset stream position to 0

                    string contentType;
                    if (Path.GetExtension(user.StudentPhoto).ToLower() == ".jpg")
                    {
                        contentType = "image/jpg";
                    }
                    else if (Path.GetExtension(user.StudentPhoto).ToLower() == ".jpeg")
                    {
                           contentType = "image/jpeg";
                    }
                    else if(Path.GetExtension(user.StudentPhoto).ToLower() == ".png")
                    {
                           contentType = "image/png";
                    }
                    else
                    {
                        return Ok(new { error = "Unsupported image type" });
                    }

                    return File(memoryStream, contentType);
                }
                return Ok(new { error = "Photo not found" });
            }
            return Ok(new { error = "User not found" });
        }

        [Authorize]
        [HttpPut("update_profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileDTO updatedUser)
        {
            var username = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name, null)?.Value;
            if (username == null)
                return Ok(new ErrorMessageDTO() { Error = "No username available" });

            username = updatedUser.Old_username;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return Ok(new ErrorMessageDTO() { Error = "User not found" });
            }

            // Actualizează datele utilizatorului cu informațiile primite.
            // Asigură-te că nu actualizezi username-ul sau alte câmpuri care nu ar trebui modificate în acest context.
            user.Firstname = updatedUser.Firstname;
            user.Lastname = updatedUser.Lastname;
            user.Cnp = updatedUser.Cnp;
            user.Sex = updatedUser.Sex;
            // ...alte câmpuri dacă este necesar

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Profil actualizat cu succes" });
            }
            catch (Exception ex)
            {
                // Log excepția folosind un serviciu de logging dacă este necesar
                return Ok(new ErrorMessageDTO() { Error = "Internal server error" });
            }
        }

        [Authorize(Roles = "admin")] // cred ca trebuia Admin cu A mare, dar vedem dupa 
        [HttpGet("get_all_users_information")]
        public async Task<ActionResult> GetAllUsersInformation()
        {
            var usersWithDocuments = await _context.Users
                .Where(u => (u.Role != "admin" && u.Role != "Admin"))
                .Select(u => new
                {
                    u.Username,
                    u.Firstname,
                    u.Lastname,
                    u.Sex,
                    u.Role,
                    u.CurrentRoomId,
                    u.AssignedRoom,
                    u.Cnp,
                    Documents = u.Documents
                        .Where(d => d.Type == ".docx" || d.Type == ".pdf")
                        .OrderByDescending(d => d.UploadTime)
                        .Select(d => new
                        {
                            d.Id,
                            d.Type,
                            UploadTime = d.UploadTime.ToString("o"), // ISO 8601 format
                            d.Status,
                            d.Feedback,
                            Url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/UploadedFiles/{d.Id}"
                        })
                        .ToList() // Convert to List to satisfy EF Core requirements
                })
                .OrderBy(u => u.Lastname).ThenBy(u => u.Firstname)
                .ToListAsync();

            return Ok(usersWithDocuments);
        }

        /*[HttpGet(Name = "GetUsernameAndRole")]
        public ActionResult GetUsernameAndRole()
        {
            // Preia claim-ul pentru username
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Preia claim-ul pentru rol
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Verifică dacă informațiile sunt disponibile
            if (username == null || role == null)
            {
                return Unauthorized("No user information available");
            }

            return Ok(new { Username = username, Role = role });
        }*/




        /* // GET: Users/Details/5
         public async Task<IActionResult> Details(Guid? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var user = await _context.Users
                 .FirstOrDefaultAsync(m => m.Id == id);
             if (user == null)
             {
                 return NotFound();
             }

             return View(user);
         }

         // GET: Users/Create
         public IActionResult Create()
         {
             return View();
         }

         // POST: Users/Create
         // To protect from overposting attacks, enable the specific properties you want to bind to.
         // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> Create([Bind("Username,Password,Role,CreatedAt,Firstname,Lastname,Cnp,CurrentRoomId,Sex,AssignedRoom,Salt,StudentPhoto,Id")] User user)
         {
             if (ModelState.IsValid)
             {
                 user.Id = Guid.NewGuid();
                 _context.Add(user);
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Index));
             }
             return View(user);
         }

         // GET: Users/Edit/5
         public async Task<IActionResult> Edit(Guid? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var user = await _context.Users.FindAsync(id);
             if (user == null)
             {
                 return NotFound();
             }
             return View(user);
         }

         // POST: Users/Edit/5
         // To protect from overposting attacks, enable the specific properties you want to bind to.
         // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> Edit(Guid id, [Bind("Username,Password,Role,CreatedAt,Firstname,Lastname,Cnp,CurrentRoomId,Sex,AssignedRoom,Salt,StudentPhoto,Id")] User user)
         {
             if (id != user.Id)
             {
                 return NotFound();
             }

             if (ModelState.IsValid)
             {
                 try
                 {
                     _context.Update(user);
                     await _context.SaveChangesAsync();
                 }
                 catch (DbUpdateConcurrencyException)
                 {
                     if (!UserExists(user.Id))
                     {
                         return NotFound();
                     }
                     else
                     {
                         throw;
                     }
                 }
                 return RedirectToAction(nameof(Index));
             }
             return View(user);
         }

         // GET: Users/Delete/5
         public async Task<IActionResult> Delete(Guid? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var user = await _context.Users
                 .FirstOrDefaultAsync(m => m.Id == id);
             if (user == null)
             {
                 return NotFound();
             }

             return View(user);
         }

         // POST: Users/Delete/5
         [HttpPost, ActionName("Delete")]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> DeleteConfirmed(Guid id)
         {
             var user = await _context.Users.FindAsync(id);
             if (user != null)
             {
                 _context.Users.Remove(user);
             }

             await _context.SaveChangesAsync();
             return RedirectToAction(nameof(Index));
         }

         private bool UserExists(Guid id)
         {
             return _context.Users.Any(e => e.Id == id);
         }*/
    }
}
