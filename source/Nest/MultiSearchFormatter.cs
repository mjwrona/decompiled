// Decompiled with JetBrains decompiler
// Type: Nest.MultiSearchFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System.Collections.Generic;

namespace Nest
{
  internal class MultiSearchFormatter : IJsonFormatter<IMultiSearchRequest>, IJsonFormatter
  {
    private const byte Newline = 10;

    public IMultiSearchRequest Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<IMultiSearchRequest>().Deserialize(ref reader, formatterResolver);
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      IMultiSearchRequest value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value?.Operations == null)
        return;
      IConnectionSettingsValues settings = formatterResolver.GetConnectionSettings();
      IMemoryStreamFactory memoryStreamFactory = settings.MemoryStreamFactory;
      IElasticsearchSerializer responseSerializer = settings.RequestResponseSerializer;
      foreach (ISearchRequest searchRequest in (IEnumerable<ISearchRequest>) value.Operations.Values)
      {
        SearchRequestParameters p = searchRequest.RequestParameters;
        string str1 = (value.Index == (Indices) null || !value.Index.Equals((object) searchRequest.Index) ? (IUrlParameter) searchRequest.Index : (IUrlParameter) null)?.GetString((IConnectionConfigurationValues) settings);
        string str2 = GetString("search_type");
        if (str2 == "query_then_fetch")
          str2 = (string) null;
        var data = new
        {
          index = (Indices) str1 != value.Index ? str1 : (string) null,
          search_type = str2,
          preference = GetString("preference"),
          routing = GetString("routing"),
          ignore_unavailable = GetString("ignore_unavailable")
        };
        writer.WriteSerialized(data, responseSerializer, (IConnectionConfigurationValues) settings);
        writer.WriteRaw((byte) 10);
        writer.WriteSerialized<ISearchRequest>(searchRequest, responseSerializer, (IConnectionConfigurationValues) settings);
        writer.WriteRaw((byte) 10);

        string GetString(string key) => p.GetResolvedQueryStringValue(key, (IConnectionConfigurationValues) settings);
      }
    }
  }
}
