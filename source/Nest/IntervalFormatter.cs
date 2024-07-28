// Decompiled with JetBrains decompiler
// Type: Nest.IntervalFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class IntervalFormatter : IJsonFormatter<Interval>, IJsonFormatter
  {
    public Interval Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.Number:
          return new Interval(Convert.ToInt64(reader.ReadDouble()));
        case JsonToken.String:
          return new Interval(reader.ReadString());
        default:
          reader.ReadNextBlock();
          return (Interval) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      Interval value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value.Unit.HasValue)
        writer.WriteString(value.ToString());
      else
        writer.WriteInt64(value.Factor);
    }
  }
}
