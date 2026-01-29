using MetaQuotes.MT5CommonAPI;
using MT5LIB.Enums;
using Newtonsoft.Json;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace MT5LIB.Models;

[ProtoContract]
public class ManagerDeal
{
    [JsonProperty("deal")]
    [Key]
    [ProtoMember(1)]
    public ulong DealId { get; set; }

    [JsonProperty("login")]
    [ProtoMember(2)]
    public ulong LoginId { get; set; }

    [JsonProperty("sl")]
    [ProtoMember(3)]
    public double Sl { get; set; }

    [JsonProperty("tp")]
    [ProtoMember(4)]
    public double Tp { get; set; }

    //[JsonProperty("fee")]
    //public double Fee { get; set; }

    [JsonProperty("dealer")]
    [ProtoMember(5)]
    public ulong Dealer { get; set; }

    [JsonProperty("type")]
    [ProtoMember(6)]
    public TradeType Type { get; set; }

    [JsonProperty("swap")]
    [ProtoMember(7)]
    public double Swap { get; set; }

    [JsonProperty("demo")]
    [ProtoMember(8)]
    public bool Demo { get; set; } 

    [JsonProperty("price")]
    [ProtoMember(9)]
    public double Price { get; set; }

    [JsonProperty("entry")]
    [ProtoMember(10)]
    public string Entry { get; set; } = string.Empty;

    [JsonProperty("order")]
    [ProtoMember(11)]
    public ulong OrderId { get; set; }

    [JsonProperty("profit")]
    [ProtoMember(12)]
    public double Profit { get; set; }

    [JsonProperty("time")]
    [ProtoMember(13)]
    public DateTime Time { get; set; }

    [JsonProperty("symbol")]
    [ProtoMember(14)]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("reason")]
    [ProtoMember(15)]
    public string Reason { get; set; } = string.Empty;

    [JsonProperty("volume")]
    [ProtoMember(16)]
    public double Volume { get; set; }

    [JsonProperty("comment")]
    [ProtoMember(17)]
    public string Comment { get; set; } = string.Empty;

    [JsonProperty("currency")]
    [ProtoMember(18)]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("position")]
    [ProtoMember(19)]
    public double Position { get; set; }

    [JsonProperty("clientName")]
    [ProtoMember(20)]
    public string ClientName { get; set; } = string.Empty;

    [JsonProperty("marketAsk")]
    [ProtoMember(21)]
    public double MarketAsk { get; set; }

    [JsonProperty("marketBid")]
    [ProtoMember(22)]
    public double MarketBid { get; set; }

    [JsonProperty("commissionFee")]
    [ProtoMember(23)]
    public double CommissionFee { get; set; }

    [JsonProperty("action")]
    [ProtoMember(24)]
    public uint Action { get;  set; }

    [JsonProperty("fee")]
    [ProtoMember(25)]
    public double Fee { get; set; }

    //[JsonProperty("group")]
    //public string Group { get; set; } = string.Empty;

    //[JsonProperty("lastName")]
    //public string LastName { get; set; } = string.Empty;
    
    //[JsonProperty("commissionFee")]
    //public CIMTDeal.EnDealAction Action { get;  set; }
}

