// Decompiled with JetBrains decompiler
// Type: Nest.RollupSearchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class RollupSearchRequest : 
    PlainRequestBase<RollupSearchRequestParameters>,
    IRollupSearchRequest,
    IRequest<RollupSearchRequestParameters>,
    IRequest
  {
    protected IRollupSearchRequest Self => (IRollupSearchRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupSearch;

    public RollupSearchRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected RollupSearchRequest()
    {
    }

    [IgnoreDataMember]
    Indices IRollupSearchRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? TotalHitsAsInteger
    {
      get => this.Q<bool?>("rest_total_hits_as_int");
      set => this.Q("rest_total_hits_as_int", (object) value);
    }

    public bool? TypedKeys
    {
      get => this.Q<bool?>("typed_keys");
      set => this.Q("typed_keys", (object) value);
    }

    public AggregationDictionary Aggregations { get; set; }

    public QueryContainer Query { get; set; }

    public int? Size { get; set; }
  }
}
