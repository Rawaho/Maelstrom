using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared
{
    public static class JsonProvider
    {
        public static T DeserialiseObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string SerialiseObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }

    public class StringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsPrimitive;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            WriteToken(JToken.FromObject(value), writer);
        }

        public override bool CanWrite => true;

        private static void WriteToken(JToken token, JsonWriter writer)
        {
            switch (token.Type)
            {
                case JTokenType.Integer:
                    writer.WriteValue(token.Value<string>());
                    break;
                case JTokenType.Array:
                {
                    writer.WriteStartArray();
                    foreach (JToken node in token)
                        WriteToken(node, writer);
                    writer.WriteEndArray();
                    break;
                }
                default:
                    token.WriteTo(writer);
                    break;
            }
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }
    }
}
