// Decompiled with JetBrains decompiler
// Type: Nest.PutDatafeedRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutDatafeedRequest : 
    PlainRequestBase<PutDatafeedRequestParameters>,
    IPutDatafeedRequest,
    IRequest<PutDatafeedRequestParameters>,
    IRequest
  {
    protected IPutDatafeedRequest Self => (IPutDatafeedRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPutDatafeed;

    public PutDatafeedRequest(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    [SerializationConstructor]
    protected PutDatafeedRequest()
    {
    }

    [IgnoreDataMember]
    Id IPutDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public bool? IgnoreThrottled
    {
      get => this.Q<bool?>("ignore_throttled");
      set => this.Q("ignore_throttled", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public AggregationDictionary Aggregations { get; set; }

    public IChunkingConfig ChunkingConfig { get; set; }

    public Time Frequency { get; set; }

    public Indices Indices { get; set; }

    public Id JobId { get; set; }

    public QueryContainer Query { get; set; }

    public Time QueryDelay { get; set; }

    public IScriptFields ScriptFields { get; set; }

    public int? ScrollSize { get; set; }

    public int? MaximumEmptySearches { get; set; }
  }
}
