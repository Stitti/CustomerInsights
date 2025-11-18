using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomerInsights.ApiService.Patching
{
    public sealed class PatchFieldConverter<T> : JsonConverter<PatchField<T>>
    {
        public override PatchField<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return PatchField<T>.From(value: default);

            T? val = JsonSerializer.Deserialize<T>(ref reader, options);
            return PatchField<T>.From(val);
        }

        public override void Write(Utf8JsonWriter writer, PatchField<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}
