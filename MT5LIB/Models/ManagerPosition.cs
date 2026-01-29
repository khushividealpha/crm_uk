using MT5LIB.Enums;
using Newtonsoft.Json;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace MT5LIB.Models;

[ProtoContract]
public class ManagerPosition
{
    [JsonProperty("comment")]
    [ProtoMember(1)]
    public string Comment { get; set; }

    [JsonProperty("id")]
    [ProtoMember(2)]
    public ulong Id { get; set; }

    [JsonProperty("loginid")]
    [ProtoMember(3)]
    public ulong LoginId { get; set; }

    [JsonProperty("positionid")]
    [Key]
    [ProtoMember(4)]
    public ulong PositionId { get; set; }

    [JsonProperty("priceCurr")]
    [ProtoMember(5)]
    public double PriceCurr { get; set; }

    [JsonProperty("priceOpen")]
    [ProtoMember(6)]
    public double PriceOpen { get; set; }

    [JsonProperty("profit")]
    [ProtoMember(7)]
    public double Profit { get; set; }

    [JsonProperty("reason")]
    [ProtoMember(8)]
    public string Reason { get; set; }

    [JsonProperty("sl")]
    [ProtoMember(9)]
    public double Sl { get; set; }

    [JsonProperty("swap")]
    [ProtoMember(10)]
    public double Swap { get; set; }

    [JsonProperty("symbol")]
    [ProtoMember(11)]
    public string Symbol { get; set; }

    [JsonProperty("time")]
    [ProtoMember(12)]
    public DateTime Time { get; set; }

    [JsonProperty("tp")]
    [ProtoMember(13)]
    public double Tp { get; set; }

    [JsonProperty("type")]
    [ProtoMember(14)]
    public TradeType Type { get; set; }

    [JsonProperty("volume")]
    [ProtoMember(15)]
    public double Volume { get; set; }

}
