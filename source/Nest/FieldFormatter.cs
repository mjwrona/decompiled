// Decompiled with JetBrains decompiler
// Type: Nest.FieldFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class FieldFormatter : 
    IJsonFormatter<Field>,
    IJsonFormatter,
    IObjectPropertyNameFormatter<Field>
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "field",
        0
      },
      {
        "boost",
        1
      },
      {
        "format",
        2
      }
    };

    public Field Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.BeginObject:
          int count = 0;
          string name = (string) null;
          double? boost = new double?();
          string format = (string) null;
          while (reader.ReadIsInObject(ref count))
          {
            ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
            int num;
            if (FieldFormatter.Fields.TryGetValue(bytes, out num))
            {
              switch (num)
              {
                case 0:
                  name = reader.ReadString();
                  continue;
                case 1:
                  boost = new double?(reader.ReadDouble());
                  continue;
                case 2:
                  format = reader.ReadString();
                  continue;
                default:
                  continue;
              }
            }
            else
              reader.ReadNextBlock();
          }
          return new Field(name, boost, format);
        case JsonToken.String:
          return new Field(reader.ReadString());
        case JsonToken.Null:
          reader.ReadNext();
          return (Field) null;
        default:
          throw new JsonParsingException(string.Format("Cannot deserialize {0} from {1}", (object) typeof (Field).FullName, (object) currentJsonToken));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      Field value,
      IJsonFormatterResolver formatterResolver)
    {
      FieldFormatter.Serialize(ref writer, value, formatterResolver, false);
    }

    private static void Serialize(
      ref JsonWriter writer,
      Field value,
      IJsonFormatterResolver formatterResolver,
      bool serializeAsString)
    {
      if (value == (Field) null)
      {
        writer.WriteNull();
      }
      else
      {
        string str = formatterResolver.GetConnectionSettings().Inferrer.Field(value);
        if (serializeAsString || string.IsNullOrEmpty(value.Format))
        {
          writer.WriteString(str);
        }
        else
        {
          writer.WriteBeginObject();
          writer.WritePropertyName("field");
          writer.WriteString(str);
          writer.WriteValueSeparator();
          writer.WritePropertyName("format");
          writer.WriteString(value.Format);
          writer.WriteEndObject();
        }
      }
    }

    public void SerializeToPropertyName(
      ref JsonWriter writer,
      Field value,
      IJsonFormatterResolver formatterResolver)
    {
      FieldFormatter.Serialize(ref writer, value, formatterResolver, true);
    }

    public Field DeserializeFromPropertyName(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return this.Deserialize(ref reader, formatterResolver);
    }
  }
}
