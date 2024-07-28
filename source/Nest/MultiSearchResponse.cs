// Decompiled with JetBrains decompiler
// Type: Nest.MultiSearchResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Nest
{
  [DataContract]
  [JsonFormatter(typeof (MultiSearchResponseFormatter))]
  public class MultiSearchResponse : ResponseBase
  {
    public MultiSearchResponse() => this.Responses = (IDictionary<string, IResponse>) new Dictionary<string, IResponse>();

    public long Took { get; set; }

    public IEnumerable<IResponse> AllResponses => this._allResponses<IResponse>();

    public override bool IsValid => base.IsValid && this.AllResponses.All<IResponse>((Func<IResponse, bool>) (b => b.IsValid));

    public int TotalResponses => !this.Responses.HasAny<KeyValuePair<string, IResponse>>() ? 0 : this.Responses.Count<KeyValuePair<string, IResponse>>();

    [JsonFormatter(typeof (VerbatimDictionaryInterfaceKeysFormatter<string, IResponse>))]
    internal IDictionary<string, IResponse> Responses { get; set; }

    public IEnumerable<IResponse> GetInvalidResponses() => this._allResponses<IResponse>().Where<IResponse>((Func<IResponse, bool>) (r => !r.IsValid));

    public ISearchResponse<T> GetResponse<T>(string name) where T : class
    {
      IResponse response;
      if (!this.Responses.TryGetValue(name, out response))
        return (ISearchResponse<T>) null;
      IElasticsearchResponse elasticsearchResponse = (IElasticsearchResponse) response;
      if (elasticsearchResponse != null)
        elasticsearchResponse.ApiCall = this.ApiCall;
      return response as ISearchResponse<T>;
    }

    public IEnumerable<ISearchResponse<T>> GetResponses<T>() where T : class => (IEnumerable<ISearchResponse<T>>) this._allResponses<SearchResponse<T>>();

    protected override void DebugIsValid(StringBuilder sb)
    {
      sb.AppendLine("# Invalid searches (inspect individual response.DebugInformation for more detail):");
      foreach (var data in this.AllResponses.Select((item, i) => new
      {
        item = item,
        i = i
      }).Where(i => !i.item.IsValid))
        sb.AppendLine(string.Format("  search[{0}]: {1}", (object) data.i, (object) data.item));
    }

    private IEnumerable<T> _allResponses<T>() where T : class, IResponse, IElasticsearchResponse
    {
      MultiSearchResponse multiSearchResponse = this;
      foreach (T obj in multiSearchResponse.Responses.Values.OfType<T>())
      {
        obj.ApiCall = multiSearchResponse.ApiCall;
        yield return obj;
      }
    }
  }
}
