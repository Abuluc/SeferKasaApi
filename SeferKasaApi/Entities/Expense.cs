using System;
namespace SeferKasaApi.Entities;

public class Expense
{
	public int id { get; set; }
	public int? TripId { get; set; }
	public Trip? Trip { get; set; }
	public string Category { get; set; } = "Other";
	public decimal Amount { get; set; }
	public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
	public string? Note { get; set; }

}
