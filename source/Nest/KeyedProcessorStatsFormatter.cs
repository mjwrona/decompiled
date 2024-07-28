// Decompiled with JetBrains decompiler
// Type: Nest.KeyedProcessorStatsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class KeyedProcessorStatsFormatter : IJsonFormatter<KeyedProcessorStats>, IJsonFormatter
  {
    public KeyedProcessorStats Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (KeyedProcessorStats) null;
      int count = 0;
      KeyedProcessorStats keyedProcessorStats = new KeyedProcessorStats();
      while (reader.ReadIsInObject(ref count))
      {
        keyedProcessorStats.Type = reader.ReadPropertyName();
        keyedProcessorStats.Statistics = formatterResolver.GetFormatter<ProcessStats>().Deserialize(ref reader, formatterResolver);
      }
      return keyedProcessorStats;
    }

    public void Serialize(
      ref JsonWriter writer,
      KeyedProcessorStats value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null || value.Type == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        writer.WritePropertyName(value.Type);
        formatterResolver.GetFormatter<ProcessStats>().Serialize(ref writer, value.Statistics, formatterResolver);
        writer.WriteEndObject();
      }
    }
  }
}
