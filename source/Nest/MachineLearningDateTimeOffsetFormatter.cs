// Decompiled with JetBrains decompiler
// Type: Nest.MachineLearningDateTimeOffsetFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Formatters;
using System;

namespace Nest
{
  internal class MachineLearningDateTimeOffsetFormatter : 
    IJsonFormatter<DateTimeOffset>,
    IJsonFormatter
  {
    public DateTimeOffset Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.Number:
          double milliseconds = reader.ReadDouble();
          return DateTimeUtil.UnixEpoch.AddMilliseconds(milliseconds);
        case JsonToken.String:
          return formatterResolver.GetFormatter<DateTimeOffset>().Deserialize(ref reader, formatterResolver);
        case JsonToken.Null:
          reader.ReadNext();
          return new DateTimeOffset();
        default:
          throw new Exception(string.Format("Cannot deserialize {0} from token {1}", (object) "DateTimeOffset", (object) currentJsonToken));
      }
    }

    public virtual void Serialize(
      ref JsonWriter writer,
      DateTimeOffset value,
      IJsonFormatterResolver formatterResolver)
    {
      ISO8601DateTimeOffsetFormatter.Default.Serialize(ref writer, value, formatterResolver);
    }
  }
}
