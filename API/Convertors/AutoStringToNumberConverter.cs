using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Convertors
{
    public class AutoStringToNumberConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // see https://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number
            switch (Type.GetTypeCode(typeToConvert))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                try
                {
                    object value = 0;
                    int i = 0;
                    double d = 0;
                    float f = 0;
                    if (typeToConvert == typeof(int))
                    {
                        int.TryParse(s, out i);
                        return i;
                    }
                    else if (typeToConvert == typeof(float))
                    {
                        float.TryParse(s, out f);
                        return f;
                    }
                    else if (typeToConvert == typeof(double))
                    {
                        double.TryParse(s, out d);
                        return d;
                    }
                    else if(typeToConvert == typeof(string))
                    {
                        return s;
                    }
                    throw new Exception($"unable to parse {s} to number");
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                if (typeToConvert == typeof(int))
                {
                    return reader.GetInt32();
                }
                else if (typeToConvert == typeof(float))
                {
                    float f = 0;
                    reader.TryGetSingle(out f);
                    return f;
                }
                else if (typeToConvert == typeof(double))
                {
                    return reader.GetDouble();
                }

            }
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                throw new Exception($"unable to parse {document.RootElement.ToString()} to number");
            }
        }


        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var str = value.ToString();             // I don't want to write int/decimal/double/...  for each case, so I just convert it to string . You might want to replace it with strong type version.
            if (int.TryParse(str, out var i))
            {
                writer.WriteNumberValue(i);
            }
            else if (double.TryParse(str, out var d))
            {
                writer.WriteNumberValue(d);
            }
            else
            {
                throw new Exception($"unable to parse {str} to number");
            }
        }
    }
}
