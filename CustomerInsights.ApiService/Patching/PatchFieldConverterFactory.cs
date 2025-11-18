using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomerInsights.ApiService.Patching
{
    public sealed class PatchFieldConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(PatchField<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type? inner = typeToConvert.GetGenericArguments()[0];
            Type? converterType = typeof(PatchFieldConverter<>).MakeGenericType(inner);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}
