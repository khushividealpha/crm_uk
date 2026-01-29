using Newtonsoft.Json;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace MT5LIB.Models;

[ProtoContract]
public class ManagerSummaryReport
{
    [JsonProperty("additional")]
    [ProtoMember(1)]
    public double Additional { get; set; }

    [JsonProperty("clientName")]
    [ProtoMember(2)]
    public string ClientName { get; set; } = string.Empty;

    [JsonProperty("commission")]
    [ProtoMember(3)]
    public double Commission { get; set; }

    [JsonProperty("credit")]
    [ProtoMember(4)]
    public double Credit { get; set; }

    [JsonProperty("curBalance")]
    [ProtoMember(5)]
    public double CurrentBalance { get; set; }

    [JsonProperty("currency")]
    [ProtoMember(6)]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("demo")]
    [ProtoMember(7)]
    public string Demo { get; set; } = string.Empty;

    [JsonProperty("deposit")]
    [ProtoMember(8)]
    public double Deposit { get; set; }

    [JsonProperty("fee")]
    [ProtoMember(9)]
    public double Fee { get; set; }

    [JsonProperty("inOut")]
    [ProtoMember(10)]
    public double InOut { get; set; }

    [JsonProperty("loginid")]
    [Key]
    [ProtoMember(11)]
    public ulong LoginId { get; set; }

    [JsonProperty("profit")]
    [ProtoMember(12)]
    public double Profit { get; set; }

    [JsonProperty("swap")]
    [ProtoMember(13)]
    public double Swap { get; set; }

    [JsonProperty("volume")]
    [ProtoMember(14)]
    public double Volume { get; set; }

    [JsonProperty("withdraw")]
    [ProtoMember(15)]
    public double Withdraw { get; set; }

    [JsonProperty("currencyDigits")]
    [ProtoMember(16)]
    public uint CurrencyDigits { get; set; }
}