using Microsoft.AspNetCore.Mvc;

using BookClub.Models;
using BookClub.Services;
using BookClub.Contexts;

namespace Server.Controllers.v1;

[ApiController]
[Route("api/v1/books")]
public class BooksController : ControllerBase
{

    private readonly UserBookClubContext _context;
    private readonly ILogger<BooksController> _logger;

    public BooksController(UserBookClubContext context, ILogger<BooksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet()]
    public IList<Book> GetAll()
    {
        using (var bookService = new BookService(_context))
        {
            return bookService.SelectAll();
        }
    }

    [HttpGet("id")]
    public Book? GetById(int id)
    {
        using (var bookService = new BookService(_context))
        {
            return bookService.Read(id);
        }
    }

}
