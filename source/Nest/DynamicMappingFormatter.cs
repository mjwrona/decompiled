// Decompiled with JetBrains decompiler
// Type: Nest.DynamicMappingFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class DynamicMappingFormatter : 
    IJsonFormatter<Union<bool, DynamicMapping>>,
    IJsonFormatter
  {
    private static readonly AutomataDictionary Values = new AutomataDictionary()
    {
      {
        "true",
        0
      },
      {
        "false",
        1
      },
      {
        "strict",
        2
      }
    };

    public void Serialize(
      ref JsonWriter writer,
      Union<bool, DynamicMapping> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        switch (value.Tag)
        {
          case 0:
            writer.WriteBoolean(value.Item1);
            break;
          case 1:
            writer.WriteString(value.Item2.GetStringValue());
            break;
        }
      }
    }

    public Union<bool, DynamicMapping> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return (Union<bool, DynamicMapping>) null;
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.String:
          ArraySegment<byte> bytes = reader.ReadStringSegmentUnsafe();
          int num;
          if (DynamicMappingFormatter.Values.TryGetValue(bytes, out num))
          {
            switch (num)
            {
              case 0:
                return new Union<bool, DynamicMapping>(true);
              case 1:
                return new Union<bool, DynamicMapping>(false);
              case 2:
                return new Union<bool, DynamicMapping>(DynamicMapping.Strict);
            }
          }
          return (Union<bool, DynamicMapping>) null;
        case JsonToken.True:
        case JsonToken.False:
          return new Union<bool, DynamicMapping>(reader.ReadBoolean());
        default:
          throw new JsonParsingException(string.Format("Cannot parse Union<bool, DynamicMapping> from token '{0}'", (object) currentJsonToken));
      }
    }
  }
}
