// Decompiled with JetBrains decompiler
// Type: Nest.TimeSpanTicksFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class TimeSpanTicksFormatter : IJsonFormatter<TimeSpan>, IJsonFormatter
  {
    public TimeSpan Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.Number:
          return new TimeSpan(reader.ReadInt64());
        case JsonToken.String:
          return TimeSpan.Parse(reader.ReadString());
        default:
          throw new Exception(string.Format("Cannot convert token of type {0} to {1}.", (object) currentJsonToken, (object) "TimeSpan"));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      TimeSpan value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteInt64(value.Ticks);
    }
  }
}
