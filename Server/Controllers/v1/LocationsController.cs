using Microsoft.AspNetCore.Mvc;

using BookClub.Models;
using BookClub.Services;
using BookClub.Contexts;

namespace Server.Controllers.v1;

[ApiController]
[Route("api/v1/locations")]
public class LocationsController : ControllerBase
{

    private readonly UserBookClubContext _context;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(UserBookClubContext context, ILogger<LocationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet()]
    public IList<Location> GetAll()
    {
        using (var locationService = new LocationService(_context))
        {
            return locationService.SelectAll();
        }
    }

    [HttpGet("id")]
    public Location? GetById(int id)
    {
        using (var locationService = new LocationService(_context))
        {
            return locationService.Read(id);
        }
    }

}
