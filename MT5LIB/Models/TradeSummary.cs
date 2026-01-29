using Newtonsoft.Json;
using ProtoBuf;

namespace MT5LIB.Models;

[ProtoContract]
public class TradeSummary
{
    [JsonProperty("login")]
    [ProtoMember(1)]
    public ulong Login { get; set; }

    [JsonProperty("totalCount")]
    [ProtoMember(2)]
    public int TotalCount { get; set; }

    [JsonProperty("profit")]
    [ProtoMember(3)]
    public double Profit { get; set; }

    [JsonProperty("balance")]
    [ProtoMember(4)]
    public double Balance { get; set; }

    [JsonProperty("aum")]
    [ProtoMember(5)]
    public double AUM { get; set; }
}
