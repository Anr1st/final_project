using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalService.Models;

namespace RentalService.Controllers;

[ApiController]
[Route("api/v1/properties")]
public class PropertiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PropertiesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}/calendar")]
    [Authorize]
    public async Task<IActionResult> GetPropertyCalendar(Guid id)
    {
        var property = await _context.Properties
            .Include(p => p.Bookings)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (property == null)
            return NotFound("Property not found.");

        var dates = new List<string>();

        foreach (var booking in property.Bookings)
        {
            if (booking.Status == BookingStatus.cancelled)
                continue;

            var date = booking.CheckIn;
            while (date < booking.CheckOut)
            {
                dates.Add(date.ToString("yyyy-MM-dd"));
                date = date.AddDays(1);
            }
        }

        dates.Sort();
        var uniqueDates = new List<string>();
        foreach (var date in dates)
        {
            if (!uniqueDates.Contains(date))
                uniqueDates.Add(date);
        }

        var response = new PropertyCalendarResponse
        {
            OccupiedDates = uniqueDates
        };

        return Ok(response);
    }

    public class PropertyCalendarResponse
    {
        public List<string> OccupiedDates { get; set; } = new();
    }
}
