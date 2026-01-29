using MT5LIB.Enums;

namespace CRMUKMTPApi.Models;

public class TransactionModel
{
    public double amount { get; set; }
    public string comment { get; set; }
    public string currency { get; set; }
    public ulong dealid { get; set; }
    public DateTime dealtime { get; set; }
    public bool demo { get; set; }
    public ulong login { get; set; }
    public string name { get; set; }
    public TradeType type { get; set; }
}
