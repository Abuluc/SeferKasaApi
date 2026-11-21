using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeferKasaApi.Data;
using SeferKasaApi.DTOs;
using SeferKasaApi.Entities;
namespace SeferKasaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ExpensesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
        {
            if (request.Amount <=0)
            {
                return BadRequest(new { message = "Masraf tutarı 0'dan büyük olmalıdır" });
                
            }
            if (request.TripId.HasValue)
            {
                var Tripexists = await _context.Trips.AnyAsync(t => t.id == request.TripId.Value);
                if (!Tripexists)
                {
                    return BadRequest(new { message = "Belirtilen seyahat bulunamadı" });
                }
                
                
            }
            var expense = new Expense
            {
                TripId = request.TripId,
                Category = string.IsNullOrWhiteSpace(request.Category) ? "Diğer" : request.Category.Trim(),
                Amount = request.Amount,
                ExpenseDate = request.ExpenseDate ?? DateTime.UtcNow,
                Note = request.Note?.Trim()
            };
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = expense.id }, expense);

        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            return Ok(expense);
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var expenses = await _context.Expenses.ToListAsync();
            var totalExpense = expenses.Sum(e => e.Amount);
            var byCategory = expenses.GroupBy(e => e.Category)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
         
            return Ok(new ExpenseSummaryResponse(totalExpense,expenses.Count, byCategory));
        }

    }
}
