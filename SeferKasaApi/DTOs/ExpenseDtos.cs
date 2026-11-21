namespace SeferKasaApi.DTOs;

public record CreateExpenseRequest(
	int? TripId,
	string Category,
	decimal Amount,
	DateTime? ExpenseDate,
	string? Note
);

public record ExpenseSummaryResponse(
	decimal TotalExpense,
	int ExpenseCount,
	Dictionary<string, decimal> ByCategory
);