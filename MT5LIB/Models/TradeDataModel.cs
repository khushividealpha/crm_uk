using Newtonsoft.Json;
using ProtoBuf;

namespace MT5LIB.Models;

[ProtoContract]
public class TradeDataModel
{
    [JsonProperty("login")]
    [ProtoMember(1)]
    public string Login { get; set; } = string.Empty;

    [JsonProperty("symbol")]
    [ProtoMember(2)]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("group")]
    [ProtoMember(3)]
    public string Group { get; set; } = string.Empty;

    [JsonProperty("firstName")]
    [ProtoMember(4)]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("lastName")]
    [ProtoMember(5)]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("totalCount")]
    [ProtoMember(6)]
    public string TotalCount { get; set; } = string.Empty;

    [JsonProperty("lastTrade")]
    [ProtoMember(7)]
    public string LastTrade { get; set; } = string.Empty;

    [JsonProperty("firstTrade")]
    [ProtoMember(8)]
    public string FirstTrade { get; set; } = string.Empty;


    [JsonProperty("profit")]
    [ProtoMember(9)]
    public string Profit { get; set; } = string.Empty;


    [JsonProperty("swap")]
    [ProtoMember(10)]
    public string Swap { get; set; } = string.Empty;


    [JsonProperty("email")]
    [ProtoMember(11)]
    public string Email { get; set; } = string.Empty;


    [JsonProperty("phone")]
    [ProtoMember(12)]
    public string Phone { get; set; } = string.Empty;

    //[JsonProperty("tag")]
    //public string Tag { get; set; } = string.Empty;
}
