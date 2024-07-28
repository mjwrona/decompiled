// Decompiled with JetBrains decompiler
// Type: Nest.NullableDateTimeOffsetEpochSecondsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class NullableDateTimeOffsetEpochSecondsFormatter : 
    IJsonFormatter<DateTimeOffset?>,
    IJsonFormatter
  {
    public DateTimeOffset? Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.Number)
      {
        reader.ReadNextBlock();
        return new DateTimeOffset?();
      }
      double seconds = reader.ReadDouble();
      return new DateTimeOffset?(DateTimeUtil.UnixEpoch.AddSeconds(seconds));
    }

    public void Serialize(
      ref JsonWriter writer,
      DateTimeOffset? value,
      IJsonFormatterResolver formatterResolver)
    {
      if (!value.HasValue)
      {
        writer.WriteNull();
      }
      else
      {
        double totalSeconds = (value.Value - DateTimeUtil.UnixEpoch).TotalSeconds;
        writer.WriteInt64((long) totalSeconds);
      }
    }
  }
}
