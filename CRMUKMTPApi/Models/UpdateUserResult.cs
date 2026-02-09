using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CRMUKMTPApi.Models
{
    public class UpdateUsersResult
    {
        public List<UpdateUserResult> Results { get; set; } = new();
    }

    public class UpdateUserResult
    {
        public ulong Mt5Id { get; set; }
        public bool Success { get; set; }
        public bool BadRequest { get; set; }
        public string Message { get; set; } = string.Empty;
    }


    public class UpdateUsersRequest
    {
        public List<UpdateUserDto> Users { get; set; } = new();
    }

    public class UpdateUserDto
    {
        [JsonProperty("mt5_id")]

        public ulong Mt5Id { get; set; }

        public Dictionary<string, string> Fields { get; set; } = new();
    }
}
