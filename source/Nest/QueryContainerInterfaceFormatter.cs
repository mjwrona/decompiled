// Decompiled with JetBrains decompiler
// Type: Nest.QueryContainerInterfaceFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System.Text;

namespace Nest
{
  internal class QueryContainerInterfaceFormatter : IJsonFormatter<IQueryContainer>, IJsonFormatter
  {
    public IQueryContainer Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return (IQueryContainer) formatterResolver.GetFormatter<QueryContainer>().Deserialize(ref reader, formatterResolver);
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      IQueryContainer value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        IRawQuery rawQuery = value.RawQuery;
        if (rawQuery != null && !rawQuery.Raw.IsNullOrEmpty() && rawQuery.IsWritable)
          writer.WriteRaw(Encoding.UTF8.GetBytes(rawQuery.Raw));
        else
          DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<IQueryContainer>().Serialize(ref writer, value, formatterResolver);
      }
    }
  }
}
