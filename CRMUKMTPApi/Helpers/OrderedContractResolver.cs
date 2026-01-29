using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CRMUKMTPApi.Helpers;

public class OrderedContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
    {
        // Get the properties
        var properties = base.CreateProperties(type, memberSerialization);

        // Order them alphabetically by PropertyName
        return properties.OrderBy(p => p.PropertyName).ToList();
    }
}
