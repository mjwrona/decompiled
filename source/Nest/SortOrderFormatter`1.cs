// Decompiled with JetBrains decompiler
// Type: Nest.SortOrderFormatter`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class SortOrderFormatter<TSortOrder> : IJsonFormatter<TSortOrder>, IJsonFormatter where TSortOrder : class, ISortOrder, new()
  {
    public TSortOrder Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return default (TSortOrder);
      }
      int count = 0;
      TSortOrder sortOrder = new TSortOrder();
      while (reader.ReadIsInObject(ref count))
      {
        sortOrder.Key = reader.ReadPropertyName();
        sortOrder.Order = formatterResolver.GetFormatter<SortOrder>().Deserialize(ref reader, formatterResolver);
      }
      return sortOrder;
    }

    public void Serialize(
      ref JsonWriter writer,
      TSortOrder value,
      IJsonFormatterResolver formatterResolver)
    {
      if ((object) value == null || value.Key == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        writer.WritePropertyName(value.Key);
        formatterResolver.GetFormatter<SortOrder>().Serialize(ref writer, value.Order, formatterResolver);
        writer.WriteEndObject();
      }
    }
  }
}
