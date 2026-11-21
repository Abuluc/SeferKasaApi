namespace SeferKasaApi.DTOs
{
    public record CreateTripRequest(
        string Title,
        DateTime StartDate,
        DateTime? EndDate,
        decimal AgreedAmount
    );
    public record TripSummaryResponse(
        int id,
        string Title,
        decimal AgreedAmount,
        decimal? CollectedAmount,
        string Status,
        decimal NetProfit,
        decimal TotalExpenses, 
        decimal RemainingReceivablet
    );
    public class TripDtos
    {
    }
}
