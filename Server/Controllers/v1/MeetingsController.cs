using Microsoft.AspNetCore.Mvc;

using BookClub.Models;
using BookClub.Services;
using BookClub.Contexts;

namespace BookClub.Controllers.v1;

[ApiController]
[Route("api/v1/meetings")]
public class MeetingsController : ControllerBase
{

    private readonly UserBookClubContext _context;
    private readonly ILogger<MeetingsController> _logger;

    public MeetingsController(UserBookClubContext context, ILogger<MeetingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet()]
    public IList<Meeting> GetAll()
    {
        using (var meetingService = new MeetingService(_context))
        {
            return meetingService.SelectAll();
        }
    }

    [HttpGet("id")]
    public Meeting? GetById(int id)
    {
        using (var meetingService = new MeetingService(_context))
        {
            return meetingService.Read(id);
        }
    }

    [HttpGet("next")]
    public Meeting? GetNext()
    {
        using (var meetingService = new MeetingService(_context))
        {
            return meetingService.NextMeeting();
        }
    }

}
