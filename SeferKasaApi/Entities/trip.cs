using System;
namespace SeferKasaApi.Entities;

public class Trip
{
	public int id { get; set; }
	public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal AgreedAmount { get; set; }
    public decimal? CollectedAmount { get; set; }
    public string Status { get; set; } = "Planned";
    public List<Expense> Expenses { get; set; } = new();
}
