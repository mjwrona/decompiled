// Decompiled with JetBrains decompiler
// Type: Nest.GeoLocationFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Nest
{
  internal class GeoLocationFormatter : IJsonFormatter<GeoLocation>, IJsonFormatter
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "lat",
        0
      },
      {
        "lon",
        1
      }
    };

    public GeoLocation Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.String:
          using (WellKnownTextTokenizer tokenizer = new WellKnownTextTokenizer((TextReader) new StringReader(reader.ReadString())))
          {
            string str = tokenizer.NextToken() == TokenType.Word ? tokenizer.TokenValue.ToUpperInvariant() : throw new GeoWKTException("Expected word but found " + tokenizer.TokenString(), tokenizer.LineNumber, tokenizer.Position);
            if (str != "POINT")
              throw new GeoWKTException("Expected POINT but found " + str, tokenizer.LineNumber, tokenizer.Position);
            if (GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word)
              return (GeoLocation) null;
            double longitude = GeoWKTReader.NextNumber(tokenizer);
            return new GeoLocation(GeoWKTReader.NextNumber(tokenizer), longitude)
            {
              Format = GeoFormat.WellKnownText
            };
          }
        case JsonToken.Null:
          reader.ReadNext();
          return (GeoLocation) null;
        default:
          int count = 0;
          double latitude = 0.0;
          double longitude1 = 0.0;
          while (reader.ReadIsInObject(ref count))
          {
            ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
            int num;
            if (GeoLocationFormatter.Fields.TryGetValue(bytes, out num))
            {
              switch (num)
              {
                case 0:
                  latitude = reader.ReadDouble();
                  continue;
                case 1:
                  longitude1 = reader.ReadDouble();
                  continue;
                default:
                  continue;
              }
            }
            else
              reader.ReadNextBlock();
          }
          return new GeoLocation(latitude, longitude1)
          {
            Format = GeoFormat.GeoJson
          };
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      GeoLocation value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        switch (value.Format)
        {
          case GeoFormat.GeoJson:
            writer.WriteBeginObject();
            writer.WritePropertyName("lat");
            writer.WriteDouble(value.Latitude);
            writer.WriteValueSeparator();
            writer.WritePropertyName("lon");
            writer.WriteDouble(value.Longitude);
            writer.WriteEndObject();
            break;
          case GeoFormat.WellKnownText:
            double num = value.Longitude;
            string str1 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            num = value.Latitude;
            string str2 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            StringBuilder stringBuilder = new StringBuilder("POINT".Length + str1.Length + str2.Length + 4).Append("POINT").Append(" (").Append(str1).Append(" ").Append(str2).Append(")");
            writer.WriteString(stringBuilder.ToString());
            break;
        }
      }
    }
  }
}
