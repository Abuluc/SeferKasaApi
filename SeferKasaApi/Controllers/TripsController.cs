using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeferKasaApi.Data;
using SeferKasaApi.DTOs;
using SeferKasaApi.Entities;

namespace SeferKasaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TripsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTripRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Başlık boş olamaz." });

        if (request.AgreedAmount <= 0)
            return BadRequest(new { message = "Anlaşılan tutar 0 veya negatif olamaz." });

        if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate)
            return BadRequest(new { message = "Bitiş tarihi başlangıç tarihinden önce olamaz." });

        var trip = new Trip
        {
            Title = request.Title.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AgreedAmount = request.AgreedAmount,
            CollectedAmount = 0,
            Status = "Planned"
        };

        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = trip.id }, trip);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var trips = await _context.Trips
            .Include(t => t.Expenses)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();

        return Ok(trips);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var trip = await _context.Trips
            .Include(t => t.Expenses)
            .FirstOrDefaultAsync(t => t.id == id);

        if (trip == null)
            return NotFound(new { message = "Sefer bulunamadı." });

        return Ok(trip);
    }

    [HttpGet("{id:int}/summary")]
    public async Task<IActionResult> GetSummary(int id)
    {
        var trip = await _context.Trips
            .Include(t => t.Expenses)
            .FirstOrDefaultAsync(t => t.id == id);

        if (trip == null)
            return NotFound(new { message = "Sefer bulunamadı." });

        var totalExpenses = trip.Expenses.Sum(e => e.Amount);
        var remainingReceivable = trip.AgreedAmount - trip.CollectedAmount;
        var netProfit = trip.CollectedAmount - totalExpenses;

        return Ok(new
        {
            trip.id,
            trip.Title,
            trip.Status,
            trip.AgreedAmount,
            trip.CollectedAmount,
            RemainingReceivable = remainingReceivable,
            TotalExpenses = totalExpenses,
            NetProfit = netProfit
        });
    }
    public record UpdateTripStatusRequest(string Status);

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTripStatusRequest request)
    {
        var validStatuses = new[] { "Planned", "InProgress", "Completed", "Cancelled" };
        if (string.IsNullOrWhiteSpace(request.Status) ||
            !validStatuses.Contains(request.Status.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Geçersiz durum. Kullanılabilir değerler: Planned, InProgress, Completed, Cancelled" });
        }

        var trip = await _context.Trips.FindAsync(id);
        if (trip is null)
            return NotFound(new { message = "Sefer bulunamadı." });

        trip.Status = validStatuses.First(s => s.Equals(request.Status.Trim(), StringComparison.OrdinalIgnoreCase));
        await _context.SaveChangesAsync();

        return Ok(new { trip.Id, trip.Title, trip.Status });
    }
}