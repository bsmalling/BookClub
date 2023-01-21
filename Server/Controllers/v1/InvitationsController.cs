using Microsoft.AspNetCore.Mvc;

using BookClub.Models;
using BookClub.Services;
using BookClub.Contexts;

namespace Server.Controllers.v1;

[ApiController]
[Route("api/v1/invitations")]
public class InvitationsController : ControllerBase
{

    private readonly UserBookClubContext _context;
    private readonly ILogger<InvitationsController> _logger;

    public InvitationsController(UserBookClubContext context, ILogger<InvitationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet()]
    public IList<Invitation> GetAll()
    {
        using (var invitationService = new InvitationService(_context))
        {
            return invitationService.SelectAll();
        }
    }

    [HttpGet("user")]
    public IList<Invitation> GetByUser(int userId)
    {
        using (var invitationService = new InvitationService(_context))
        {
            return invitationService.ReadByUser(userId, false);
        }
    }

    [HttpGet("code")]
    public Invitation? GetByCode(string code)
    {
        using (var invitationService = new InvitationService(_context))
        {
            return invitationService.Read(code);
        }
    }

}
