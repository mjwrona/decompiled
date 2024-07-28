// Decompiled with JetBrains decompiler
// Type: Nest.QueryContainerCollectionFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class QueryContainerCollectionFormatter : 
    IJsonFormatter<IEnumerable<QueryContainer>>,
    IJsonFormatter
  {
    private static readonly QueryContainerFormatter QueryContainerFormatter = new QueryContainerFormatter();
    private static readonly QueryContainerInterfaceFormatter QueryContainerInterfaceFormatter = new QueryContainerInterfaceFormatter();

    public IEnumerable<QueryContainer> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          return (IEnumerable<QueryContainer>) new List<QueryContainer>()
          {
            QueryContainerCollectionFormatter.QueryContainerFormatter.Deserialize(ref reader, formatterResolver)
          };
        case JsonToken.BeginArray:
          int count = 0;
          List<QueryContainer> queryContainerList = new List<QueryContainer>();
          while (reader.ReadIsInArray(ref count))
            queryContainerList.Add(QueryContainerCollectionFormatter.QueryContainerFormatter.Deserialize(ref reader, formatterResolver));
          return (IEnumerable<QueryContainer>) queryContainerList;
        default:
          reader.ReadNextBlock();
          return (IEnumerable<QueryContainer>) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      IEnumerable<QueryContainer> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginArray();
        using (IEnumerator<QueryContainer> enumerator = value.GetEnumerator())
        {
          bool flag = false;
          while (enumerator.MoveNext())
          {
            if (enumerator.Current != null && enumerator.Current.IsWritable)
            {
              if (flag)
                writer.WriteValueSeparator();
              QueryContainerCollectionFormatter.QueryContainerInterfaceFormatter.Serialize(ref writer, (IQueryContainer) enumerator.Current, formatterResolver);
              flag = true;
            }
          }
        }
        writer.WriteEndArray();
      }
    }
  }
}
