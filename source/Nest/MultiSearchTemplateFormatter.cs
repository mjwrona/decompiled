// Decompiled with JetBrains decompiler
// Type: Nest.MultiSearchTemplateFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class MultiSearchTemplateFormatter : 
    IJsonFormatter<IMultiSearchTemplateRequest>,
    IJsonFormatter
  {
    private const byte Newline = 10;

    public IMultiSearchTemplateRequest Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return (IMultiSearchTemplateRequest) formatterResolver.GetFormatter<MultiSearchTemplateRequest>().Deserialize(ref reader, formatterResolver);
    }

    public void Serialize(
      ref JsonWriter writer,
      IMultiSearchTemplateRequest value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value?.Operations == null)
        return;
      IConnectionSettingsValues settings = formatterResolver.GetConnectionSettings();
      IMemoryStreamFactory memoryStreamFactory = settings.MemoryStreamFactory;
      IElasticsearchSerializer responseSerializer = settings.RequestResponseSerializer;
      foreach (ISearchTemplateRequest searchTemplateRequest in (IEnumerable<ISearchTemplateRequest>) value.Operations.Values)
      {
        SearchTemplateRequestParameters p = searchTemplateRequest.RequestParameters;
        IUrlParameter index = value.Index == (Indices) null || !value.Index.Equals((object) searchTemplateRequest.Index) ? (IUrlParameter) searchTemplateRequest.Index : (IUrlParameter) null;
        string str = GetString("search_type");
        if (str == "query_then_fetch")
          str = (string) null;
        var data = new
        {
          index = index?.GetString((IConnectionConfigurationValues) settings),
          search_type = str,
          preference = GetString("preference"),
          routing = GetString("routing"),
          ignore_unavailable = GetString("ignore_unavailable")
        };
        writer.WriteSerialized(data, responseSerializer, (IConnectionConfigurationValues) settings);
        writer.WriteRaw((byte) 10);
        writer.WriteSerialized<ISearchTemplateRequest>(searchTemplateRequest, responseSerializer, (IConnectionConfigurationValues) settings);
        writer.WriteRaw((byte) 10);

        string GetString(string key) => p.GetResolvedQueryStringValue(key, (IConnectionConfigurationValues) settings);
      }
    }
  }
}
