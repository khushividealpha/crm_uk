using CRMUKMTPApi.Helpers;
using MetaQuotes.MT5CommonAPI;
using MT5LIB.Enums;
using Newtonsoft.Json;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace MT5LIB.Models;
[ProtoContract]
public class ManagerOrder
{
    [JsonProperty("sl")]
    [ProtoMember(1)]
    public double Sl { get; set; }

    [JsonProperty("tp")]
    [ProtoMember(2)]
    public double Tp { get; set; }

    [JsonProperty("login")]
    [ProtoMember(3)]
    public ulong Login { get; set; }

    [JsonProperty("order")]
    [Key]
    [ProtoMember(4)]
    public ulong OrderId { get; set; }

    [JsonProperty("demo")]
    [ProtoMember(5)]
    public bool Demo { get; set; }

    [JsonProperty("type")]
    [ProtoMember(6)]
    public TradeType Type { get; set; }

    [JsonProperty("position")]
    [ProtoMember(7)]
    public double Position { get; set; }

    [JsonProperty("price")]
    [ProtoMember(8)]
    public double Price { get; set; }

    [JsonProperty("reason")]
    [ProtoMember(9)]
    public string Reason { get; set; } = string.Empty;

    [JsonProperty("state")]
    [ProtoMember(10)]
    public string State { get; set; } = string.Empty;

    [JsonProperty("symbol")]
    [ProtoMember(11)]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("comment")]
    [ProtoMember(12)]
    public string Comment { get; set; } = string.Empty;

    [JsonProperty("clientName")]
    [ProtoMember(13)]
    public string ClientName { get; set; } = string.Empty;

    [JsonProperty("doneTime")]
    [JsonConverter(typeof(CustomDateTimeConverter))]
    [ProtoMember(14)]
    public DateTime DoneTime { get; set; }

    [JsonProperty("setupTime")]
    [JsonConverter(typeof(CustomDateTimeConverter))]
    [ProtoMember(15)]
    public DateTime SetupTime { get; set; }

    [JsonProperty("volumeTotal")]
    [ProtoMember(16)]
    public double VolumeTotal { get; set; }

    [JsonProperty("volumeFilled")]
    [ProtoMember(17)]
    public double VolumeFilled { get; set; }
}
