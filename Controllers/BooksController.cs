using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApplication.Database;
using LibraryApplication.Models;
using LibraryApplication.DTOs;
using LibraryApplication.Services;

namespace LibraryApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public BooksController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            bool isAuthorized = _jwtService.checkAuthorization(Request);
            if (!isAuthorized) return Unauthorized();

            return await _context.Books
                .Include(el => el.Author)
                .Include(el => el.Genre)
                .ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(long id)
        {
            bool isAuthorized = _jwtService.checkAuthorization(Request);
            if (!isAuthorized) return Unauthorized();

            Book book = await _context.Books
                .Where(el => el.ID == id)
                .Include(el => el.Author)
                .Include(el => el.Genre)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditBook(long id, BookDto book)
        {
            if (id != book.ID)
            {
                return NotFound();
            }

            bool isAuthorized = _jwtService.checkAuthorization(Request);
            if (!isAuthorized) return Unauthorized();

            Book editedBook = await _context.Books.Where(el => el.ID == id)
                .Include(el => el.Author)
                .Include(el => el.Genre)
                .FirstOrDefaultAsync();

            if (editedBook == null) return NotFound();

            editedBook.Name = book.Name;

            //Check if genre was changed
            if(editedBook.Genre.ID != book.GenreId)
            {
                Genre newGenre = await _context.Genres.Where(el => el.ID == book.GenreId).FirstOrDefaultAsync();
                if (newGenre == null) return NotFound();

                editedBook.Genre = newGenre;
            }

            //Check if author was changed
            if(editedBook.Author.ID != book.AuthorId)
            {
                Author newAuthor = await _context.Authors.Where(el => el.ID == book.AuthorId).FirstOrDefaultAsync();
                if (newAuthor == null) return NotFound();

                editedBook.Author = newAuthor;
            }

            _context.Entry(editedBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult> AddBook(BookDto book)
        {
            bool isAuthorized = _jwtService.checkAuthorization(Request);
            if (!isAuthorized) return Unauthorized();

            Book createdBook = new Book() { Name = book.Name, Year = book.Year };

            Author relatedAuthor = await _context.Authors.Where(author => author.ID == book.AuthorId).FirstOrDefaultAsync();
            if (relatedAuthor == null) return StatusCode(422);
            createdBook.Author = relatedAuthor;

            Genre relatedGenre = await _context.Genres.Where(el => el.ID == book.GenreId).FirstOrDefaultAsync();
            if (relatedGenre == null) return StatusCode(422);
            createdBook.Genre = relatedGenre;

            _context.Entry(createdBook).State = EntityState.Added;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(503);
            }

            return Ok();
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(long id)
        {
            bool isAuthorized = _jwtService.checkAuthorization(Request);
            if (!isAuthorized) return Unauthorized();

            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(503);
            }

            return Ok();
        }

        private bool BookExists(long id)
        {
            return _context.Books.Any(e => e.ID == id);
        }
    }
}
