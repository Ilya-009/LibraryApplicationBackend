using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryApplication.Database;
using LibraryApplication.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LibraryApplication.Services;
using Microsoft.AspNetCore.Http;

namespace LibraryApplication.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthenticationController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: api/auth
        [HttpPost]
        public async Task<ActionResult> RegisterUser(DTOs.UserDto registerUser)
        {
            //Check user with same email already exists
            bool userExists = _context.Users.Any(el => el.Email == registerUser.Email);

            if (userExists)
            {
                return BadRequest(new { message = "Пользователь с таким email уже существует!" });
            }

            User user = new User()
            {
                Email = registerUser.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUser.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(DTOs.UserDto userDto)
        {
            User user = await _context.Users.Where(el => el.Email == userDto.Email).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest(new { message = "Пользователя с таким email не существует!" });

            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
                return BadRequest(new { message = "Неправильный пароль!" });

            string jwt = _jwtService.generageToken(user.ID);

            //Set jwt to client cookie
            Response.Cookies.Append("jwt", jwt, new CookieOptions()
            {
                HttpOnly = true,
                IsEssential = true
            });

            return Ok();
        }
    }
}
