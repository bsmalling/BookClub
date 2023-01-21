using Microsoft.AspNetCore.Mvc;

using BookClub.Models;
using BookClub.Services;
using BookClub.Contexts;

namespace Server.Controllers.v1;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{

    private readonly UserBookClubContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserBookClubContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet()]
    public IList<User> GetAll()
    {
        using (var userService = new UserService(_context))
        {
            return userService.SelectAll();
        }
    }

    [HttpGet("id")]
    public User? GetById(int id)
    {
        using (var userService = new UserService(_context))
        {
            return userService.Read(id);
        }
    }

}
