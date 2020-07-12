using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test.Utilities
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateTime = reader.GetString();
            return DateTime.Parse(dateTime);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
