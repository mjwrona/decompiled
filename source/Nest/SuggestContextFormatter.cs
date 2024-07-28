// Decompiled with JetBrains decompiler
// Type: Nest.SuggestContextFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class SuggestContextFormatter : IJsonFormatter<ISuggestContext>, IJsonFormatter
  {
    private static readonly AutomataDictionary ContextTypes = new AutomataDictionary()
    {
      {
        "geo",
        0
      },
      {
        "category",
        1
      }
    };

    public ISuggestContext Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
      {
        reader.ReadNext();
        return (ISuggestContext) null;
      }
      ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
      JsonReader reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      int count = 0;
      ArraySegment<byte> bytes = new ArraySegment<byte>();
      while (reader1.ReadIsInObject(ref count))
      {
        if (reader1.ReadPropertyName() == "type")
        {
          bytes = reader1.ReadStringSegmentRaw();
          break;
        }
        reader1.ReadNextBlock();
      }
      reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      int num;
      if (SuggestContextFormatter.ContextTypes.TryGetValue(bytes, out num))
      {
        if (num == 0)
          return (ISuggestContext) SuggestContextFormatter.Deserialize<GeoSuggestContext>(ref reader1, formatterResolver);
        if (num == 1)
          return (ISuggestContext) SuggestContextFormatter.Deserialize<CategorySuggestContext>(ref reader1, formatterResolver);
      }
      return (ISuggestContext) SuggestContextFormatter.Deserialize<CategorySuggestContext>(ref reader1, formatterResolver);
    }

    public void Serialize(
      ref JsonWriter writer,
      ISuggestContext value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else
        formatterResolver.GetFormatter<object>().Serialize(ref writer, (object) value, formatterResolver);
    }

    private static TContext Deserialize<TContext>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TContext : ISuggestContext
    {
      return formatterResolver.GetFormatter<TContext>().Deserialize(ref reader, formatterResolver);
    }
  }
}
