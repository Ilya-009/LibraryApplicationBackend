using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApplication.Database;
using LibraryApplication.Models;
using LibraryApplication.Services;

namespace LibraryApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public GenresController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: api/Genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            bool isAuthorized = _jwtService.checkAuthorization(Request);
            if (!isAuthorized) return Unauthorized();

            return await _context.Genres.ToListAsync();
        }
    }
}
