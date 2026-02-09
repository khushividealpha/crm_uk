namespace CRMUKMTPApi.Models
{
    public class FirstDepositRequestModel
    {
     
            public List<ulong>mt5Id { get; set; }
        
        
    }
    public class FirstDepositResponse
    {
        public Dictionary<ulong, FirstDepositData> Data { get; set; } = new();
        public string Status { get; set; } = "success";
    }

    public class FirstDepositData
    {
        public DateTime AccountCreatedTime { get; set; }
        public double FirstDepositAmount { get; set; }
        public string FirstDepositMethod { get; set; }
        public DateTime FirstDepositTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastTradeTime { get; set; }
        public ulong Mt5Id { get; set; }
    }

}
