

using Newtonsoft.Json;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT5LIB.Models;

[ProtoContract]
public class ManagerDailyReport
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [ProtoMember(1)]
    public int id { get; set; }

    [JsonProperty("balance")]
    [ProtoMember(2)]
    public double Balance { get; set; }

    [JsonProperty("clientName")]
    [ProtoMember(3)]
    public string ClientName { get; set; }

    [JsonProperty("closedPL")]
    [ProtoMember(4)]
    public double ClosedPL { get; set; }

    [JsonProperty("credit")]
    [ProtoMember(5)]
    public double Credit { get; set; }

    [JsonProperty("currency")]
    [ProtoMember(6)]
    public string Currency { get; set; }

    [JsonProperty("demo")]
    [ProtoMember(7)]
    public bool Demo { get; set; }

    [JsonProperty("deposit")]
    [ProtoMember(8)]
    public double Deposit { get; set; }

    [JsonProperty("email")]
    [ProtoMember(9)]
    public string Email { get; set; }

    [JsonProperty("equity")]
    [ProtoMember(10)]
    public double Equity { get; set; }

    [JsonProperty("floatingPL")]
    [ProtoMember(11)]
    public double FloatingPL { get; set; }

    [JsonProperty("group")]
    [ProtoMember(12)]
    public string Group { get; set; }

    [JsonProperty("loginid")]
    [ProtoMember(13)]
    public ulong LoginId { get; set; }

    [JsonProperty("margin")]
    [ProtoMember(14)]
    public double Margin { get; set; }

    [JsonProperty("marginFree")]
    [ProtoMember(15)]
    public double MarginFree { get; set; }

    [JsonProperty("prevBalance")]
    [ProtoMember(16)]
    public double PrevBalance { get; set; }

    [JsonProperty("time")]
    [ProtoMember(17)]
    public DateTime Date { get; set; }
}