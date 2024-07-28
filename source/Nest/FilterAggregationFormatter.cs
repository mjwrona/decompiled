// Decompiled with JetBrains decompiler
// Type: Nest.FilterAggregationFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class FilterAggregationFormatter : IJsonFormatter<IFilterAggregation>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      IFilterAggregation value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value?.Filter == null || !value.Filter.IsWritable)
      {
        writer.WriteBeginObject();
        writer.WriteEndObject();
      }
      else
        formatterResolver.GetFormatter<QueryContainer>().Serialize(ref writer, value.Filter, formatterResolver);
    }

    public IFilterAggregation Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (IFilterAggregation) null;
      QueryContainer queryContainer = formatterResolver.GetFormatter<QueryContainer>().Deserialize(ref reader, formatterResolver);
      return (IFilterAggregation) new FilterAggregation()
      {
        Filter = queryContainer
      };
    }
  }
}
