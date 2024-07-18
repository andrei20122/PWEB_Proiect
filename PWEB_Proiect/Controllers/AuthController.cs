using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using PWEB_Proiect.DTOs;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace PWEB_Proiect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("check_credentials")]
        public ActionResult<LogInResponseDTO> LogIn([FromBody] LogInRequestDTO logInRequest)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == logInRequest.Username);
            if (user == null)
                return Ok(new ErrorMessageDTO() { Error = "Incorrect credentials"});

            byte[] hashBytes = Convert.FromBase64String(user.Password);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (var pbkdf2 = new Rfc2898DeriveBytes(logInRequest.Password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                for (int i = 0; i < 20; i++)
                    if (hashBytes[i + 16] != hash[i])
                        return Ok(new ErrorMessageDTO() { Error = "Invalid username or password"});
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: creds);

            LogInResponseDTO response = new()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("get_role")]
        public ActionResult GetRole()
        {
            var role = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role, null)?.Value;
            if (role == null)
                return Ok(new ErrorMessageDTO() { Error = "No role available"});
            
            return Ok(new { Role = role });
        }

        [Authorize]
        [HttpGet("get_username")]
        public ActionResult GetUsername()
        {

            var username = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name, null)?.Value;
            if (username == null)
                return Ok(new ErrorMessageDTO() { Error = "No username available" });

            return Ok(new { Username = username });
        }

        [Authorize]
        [HttpGet("validate_token")]
        public ActionResult ValidateToken()
        {
            return Ok(new {message = "aa"});
        }

    }
}
