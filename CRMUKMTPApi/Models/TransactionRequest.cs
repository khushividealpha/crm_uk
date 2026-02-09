namespace CRMUKMTPApi.Models
{
    public class TransactionRequest
    {
        public ulong Mt5Id { get; set; }
        public double Amount { get; set; }
        public string? Remarks { get; set; }
        public bool IsDeposit { get; set; }
    }
    public class DepositResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ulong DealId { get; set; }
    }
    public class InternalTranReuest
    {
        public ulong From { get; set; }
        public ulong To { get; set; }
        public double Amount { get; set; }
        public string? FromComment { get; set; }
        public string? ToComment { get; set; }
    }
}
