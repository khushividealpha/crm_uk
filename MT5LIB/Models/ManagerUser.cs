

using Newtonsoft.Json;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace MT5LIB.Models;

[ProtoContract]
public class ManagerUser
{
    [JsonProperty("client")]
    [ProtoMember(1)]
    public ulong Client { get; set; }

    [JsonProperty("city")]
    [ProtoMember(2)]
    public string City { get; set; } = "-";

    [JsonProperty("loginid")]
    [Key]
    [ProtoMember(3)]
    public ulong LoginId { get; set; }

    [JsonProperty("email")]
    [ProtoMember(4)]
    public string Email { get; set; } = "-";

    [JsonProperty("name")]
    [ProtoMember(5)]
    public string Name { get; set; } = "-";

    [JsonProperty("group")]
    [ProtoMember(6)]
    public string Group { get; set; } = "-";

    [JsonProperty("phone")]
    [ProtoMember(7)]
    public string Phone { get; set; } = "-";

    [JsonProperty("margin")]
    [ProtoMember(8)]
    public double Margin { get; set; }

    [JsonProperty("equity")]
    [ProtoMember(9)]
    public double Equity { get; set; }

    [JsonProperty("status")]
    [ProtoMember(10)]
    public string Status { get; set; } = "-";

    [JsonProperty("credit")]
    [ProtoMember(11)]
    public double Credit { get; set; }

    [JsonProperty("zipcode")]
    [ProtoMember(12)]
    public string Zipcode { get; set; } = "-";

    [JsonProperty("address")]
    [ProtoMember(13)]
    public string Address { get; set; } = "-";

    [JsonProperty("balance")]
    [ProtoMember(14)]
    public double Balance { get; set; }

    [JsonProperty("comment")]
    [ProtoMember(15)]
    public string Comment { get; set; } = "-";

    [JsonProperty("country")]
    [ProtoMember(16)]
    public string Country { get; set; } = "-";

    [JsonProperty("leverage")]
    [ProtoMember(17)]
    public double Leverage { get; set; }

    [JsonProperty("created")]
    [ProtoMember(18)]
    public DateTime? Created { get; set; }

    [JsonProperty("lastaccess")]
    [ProtoMember(19)]
    public string LastAccess { get; set; } = "-";

    [JsonProperty("freemargin")]
    [ProtoMember(20)]
    public double FreeMargin { get; set; }

    [JsonProperty("demo")]
    [ProtoMember(21)]
    public bool Demo { get; set; }

    [JsonProperty("groupCurrency")]
    [ProtoMember(22)]
    public string GroupCurrency { get; set; } = string.Empty;

    [JsonProperty("currencyDigits")]
    [ProtoMember(23)]
    public uint CurrencyDigits { get; internal set; }

    [JsonProperty("lastName")]
    [ProtoMember(24)]
    public string LastName { get; set; } = string.Empty;
    [JsonProperty("enabled")]
    [ProtoMember(25)]
    public bool Enabled { get; set; }
}

