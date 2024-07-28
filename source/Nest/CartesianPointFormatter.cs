// Decompiled with JetBrains decompiler
// Type: Nest.CartesianPointFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Text;

namespace Nest
{
  internal class CartesianPointFormatter : IJsonFormatter<CartesianPoint>, IJsonFormatter
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "x",
        0
      },
      {
        "y",
        1
      },
      {
        "z",
        2
      }
    };

    public void Serialize(
      ref JsonWriter writer,
      CartesianPoint value,
      IJsonFormatterResolver formatterResolver)
    {
      if ((object) value == null)
      {
        writer.WriteNull();
      }
      else
      {
        switch (value.Format)
        {
          case ShapeFormat.Object:
            writer.WriteBeginObject();
            writer.WritePropertyName("x");
            writer.WriteSingle(value.X);
            writer.WriteValueSeparator();
            writer.WritePropertyName("y");
            writer.WriteSingle(value.Y);
            writer.WriteEndObject();
            break;
          case ShapeFormat.Array:
            writer.WriteBeginArray();
            writer.WriteSingle(value.X);
            writer.WriteValueSeparator();
            writer.WriteSingle(value.Y);
            writer.WriteEndArray();
            break;
          case ShapeFormat.WellKnownText:
            writer.WriteQuotation();
            writer.WriteRaw(Encoding.UTF8.GetBytes("POINT"));
            writer.WriteRaw((byte) 32);
            writer.WriteRaw((byte) 40);
            writer.WriteSingle(value.X);
            writer.WriteRaw((byte) 32);
            writer.WriteSingle(value.Y);
            writer.WriteRaw((byte) 41);
            writer.WriteQuotation();
            break;
          case ShapeFormat.String:
            writer.WriteQuotation();
            writer.WriteSingle(value.X);
            writer.WriteValueSeparator();
            writer.WriteSingle(value.Y);
            writer.WriteQuotation();
            break;
        }
      }
    }

    public CartesianPoint Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.BeginObject:
          int count1 = 0;
          CartesianPoint cartesianPoint1 = new CartesianPoint()
          {
            Format = ShapeFormat.Object
          };
          while (reader.ReadIsInObject(ref count1))
          {
            ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
            int num1;
            if (!CartesianPointFormatter.Fields.TryGetValue(segment, out num1))
              throw new JsonParsingException("Unknown property " + segment.Utf8String() + " when parsing CartesianPoint");
            switch (num1)
            {
              case 0:
                cartesianPoint1.X = reader.ReadSingle();
                continue;
              case 1:
                cartesianPoint1.Y = reader.ReadSingle();
                continue;
              case 2:
                double num2 = (double) reader.ReadSingle();
                continue;
              default:
                continue;
            }
          }
          return cartesianPoint1;
        case JsonToken.BeginArray:
          int count2 = 0;
          CartesianPoint cartesianPoint2 = new CartesianPoint()
          {
            Format = ShapeFormat.Array
          };
          while (reader.ReadIsInArray(ref count2))
          {
            switch (count2)
            {
              case 1:
                cartesianPoint2.X = reader.ReadSingle();
                continue;
              case 2:
                cartesianPoint2.Y = reader.ReadSingle();
                continue;
              case 3:
                double num = (double) reader.ReadSingle();
                continue;
              default:
                throw new JsonParsingException(string.Format("Expected 2 or 3 coordinates but found {0}", (object) count2));
            }
          }
          return cartesianPoint2;
        case JsonToken.String:
          string str = reader.ReadString();
          return str.IndexOf(",", StringComparison.InvariantCultureIgnoreCase) <= -1 ? CartesianPoint.FromWellKnownText(str) : CartesianPoint.FromCoordinates(str);
        default:
          throw new JsonParsingException(string.Format("Unexpected token type {0} when parsing {1}", (object) currentJsonToken, (object) "CartesianPoint"));
      }
    }
  }
}
