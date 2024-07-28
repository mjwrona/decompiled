// Decompiled with JetBrains decompiler
// Type: Nest.TermsIncludeFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class TermsIncludeFormatter : IJsonFormatter<TermsInclude>, IJsonFormatter
  {
    private static readonly AutomataDictionary AutomataDictionary = new AutomataDictionary()
    {
      {
        "partition",
        0
      },
      {
        "num_partitions",
        1
      }
    };

    public TermsInclude Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.BeginObject:
          long partition = 0;
          long numberOfPartitions = 0;
          int count = 0;
          while (reader.ReadIsInObject(ref count))
          {
            ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
            int num;
            if (TermsIncludeFormatter.AutomataDictionary.TryGetValue(bytes, out num))
            {
              switch (num)
              {
                case 0:
                  partition = reader.ReadInt64();
                  continue;
                case 1:
                  numberOfPartitions = reader.ReadInt64();
                  continue;
                default:
                  continue;
              }
            }
          }
          return new TermsInclude(partition, numberOfPartitions);
        case JsonToken.BeginArray:
          return new TermsInclude(formatterResolver.GetFormatter<IEnumerable<string>>().Deserialize(ref reader, formatterResolver));
        case JsonToken.String:
          return new TermsInclude(reader.ReadString());
        case JsonToken.Null:
          reader.ReadNext();
          return (TermsInclude) null;
        default:
          throw new Exception(string.Format("Unexpected token {0} when deserializing {1}", (object) currentJsonToken, (object) "TermsInclude"));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      TermsInclude value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else if (value.Values != null)
      {
        formatterResolver.GetFormatter<IEnumerable<string>>().Serialize(ref writer, value.Values, formatterResolver);
      }
      else
      {
        long? nullable = value.Partition;
        if (nullable.HasValue)
        {
          nullable = value.NumberOfPartitions;
          if (nullable.HasValue)
          {
            writer.WriteBeginObject();
            writer.WritePropertyName("partition");
            ref JsonWriter local1 = ref writer;
            nullable = value.Partition;
            long num1 = nullable.Value;
            local1.WriteInt64(num1);
            writer.WriteValueSeparator();
            writer.WritePropertyName("num_partitions");
            ref JsonWriter local2 = ref writer;
            nullable = value.NumberOfPartitions;
            long num2 = nullable.Value;
            local2.WriteInt64(num2);
            writer.WriteEndObject();
            return;
          }
        }
        writer.WriteString(value.Pattern);
      }
    }
  }
}
