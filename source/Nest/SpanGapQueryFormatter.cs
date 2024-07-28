// Decompiled with JetBrains decompiler
// Type: Nest.SpanGapQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class SpanGapQueryFormatter : IJsonFormatter<ISpanGapQuery>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      ISpanGapQuery value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null || SpanGapQuery.IsConditionless(value))
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        Inferrer inferrer = formatterResolver.GetConnectionSettings().Inferrer;
        writer.WritePropertyName(inferrer.Field(value.Field));
        writer.WriteInt32(value.Width.Value);
        writer.WriteEndObject();
      }
    }

    public ISpanGapQuery Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
      {
        reader.ReadNext();
        return (ISpanGapQuery) null;
      }
      int count = 0;
      SpanGapQuery spanGapQuery = new SpanGapQuery();
      while (reader.ReadIsInObject(ref count))
      {
        if (count <= 1)
        {
          spanGapQuery.Field = (Field) reader.ReadPropertyName();
          spanGapQuery.Width = new int?(reader.ReadInt32());
        }
      }
      return (ISpanGapQuery) spanGapQuery;
    }
  }
}
