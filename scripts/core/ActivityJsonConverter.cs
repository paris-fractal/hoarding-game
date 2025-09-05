using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace hoardinggame.Core
{
    public class ActivityJsonConverter : JsonConverter<Activity>
    {
        public override Activity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("$type", out var typeProperty))
            {
                throw new JsonException("Missing $type property for Activity deserialization");
            }

            var typeName = typeProperty.GetString();
            return typeName switch
            {
                nameof(RotatePlayerActivity) => JsonSerializer.Deserialize<RotatePlayerActivity>(root.GetRawText(), options),
                nameof(LockInputActivity) => JsonSerializer.Deserialize<LockInputActivity>(root.GetRawText(), options),
                "TestActivity" => JsonSerializer.Deserialize<hoardinggame.Core.Tests.TestActivity>(root.GetRawText(), options),
                _ => throw new JsonException($"Unknown Activity type: {typeName}")
            };
        }

        public override void Write(Utf8JsonWriter writer, Activity value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.GetType().Name);
            
            var jsonString = JsonSerializer.Serialize(value, value.GetType(), options);
            using var jsonDoc = JsonDocument.Parse(jsonString);
            foreach (var property in jsonDoc.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }
            
            writer.WriteEndObject();
        }
    }
}