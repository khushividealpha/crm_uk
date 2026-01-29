using MT5LIB.Enums;
using Newtonsoft.Json;
using ProtoBuf;

namespace MT5LIB.Models;

[ProtoContract]
public class ManagerTransaction
{
    [JsonProperty("amount")]
    [ProtoMember(1)]
    public double Amount { get; set; }

    [JsonProperty("comment")]
    [ProtoMember(2)]
    public string Comment { get; set; } = string.Empty;

    [JsonProperty("currency")]
    [ProtoMember(3)]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("dealid")]
    [ProtoMember(4)]
    public ulong DealId { get; set; }

    [JsonProperty("dealtime")]
    [ProtoMember(5)]
    public DateTime? DealTime { get; set; }

    [JsonProperty("demo")]
    [ProtoMember(6)]
    public bool Demo { get; set; } 

    [JsonProperty("login")]
    [ProtoMember(7)]
    public ulong Login { get; set; }

    [JsonProperty("name")]
    [ProtoMember(8)]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("type")]
    [ProtoMember(9)]
    public TradeType Type { get; set; } 
}

