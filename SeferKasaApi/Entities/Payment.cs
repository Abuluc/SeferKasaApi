namespace SeferKasaApi.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public Trip? Trip { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; } = DateTime.UtcNow;
        public string? PaymentMethod { get; set; } = "BankTransfer";
        public string? Note { get; set; }
    }
}
