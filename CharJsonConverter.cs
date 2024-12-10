using System.Text.Json;
using System.Text.Json.Serialization;

namespace GokstadFriidrettsforeningAPI;

public class CharJsonConverter : JsonConverter<char>
{
    public override char Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value != null && value.Length == 1 ? value[0] : '\0';
    }

    public override void Write(Utf8JsonWriter writer, char value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
