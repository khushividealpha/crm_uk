using Newtonsoft.Json;
using System.Globalization;

namespace CRMUKMTPApi.Helpers;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd HH:mm:ss.fff";

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString(Format));
    }

    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        return DateTime.ParseExact((string)reader.Value, Format, CultureInfo.InvariantCulture);
    }
}
