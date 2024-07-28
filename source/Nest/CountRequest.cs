// Decompiled with JetBrains decompiler
// Type: Nest.CountRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class CountRequest : 
    PlainRequestBase<CountRequestParameters>,
    ICountRequest,
    IRequest<CountRequestParameters>,
    IRequest
  {
    protected ICountRequest Self => (ICountRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceCount;

    public CountRequest()
    {
    }

    public CountRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices ICountRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public bool? AnalyzeWildcard
    {
      get => this.Q<bool?>("analyze_wildcard");
      set => this.Q("analyze_wildcard", (object) value);
    }

    public string Analyzer
    {
      get => this.Q<string>("analyzer");
      set => this.Q("analyzer", (object) value);
    }

    public Elasticsearch.Net.DefaultOperator? DefaultOperator
    {
      get => this.Q<Elasticsearch.Net.DefaultOperator?>("default_operator");
      set => this.Q("default_operator", (object) value);
    }

    public string Df
    {
      get => this.Q<string>("df");
      set => this.Q("df", (object) value);
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

    public bool? Lenient
    {
      get => this.Q<bool?>("lenient");
      set => this.Q("lenient", (object) value);
    }

    public double? MinScore
    {
      get => this.Q<double?>("min_score");
      set => this.Q("min_score", (object) value);
    }

    public string Preference
    {
      get => this.Q<string>("preference");
      set => this.Q("preference", (object) value);
    }

    public string QueryOnQueryString
    {
      get => this.Q<string>("q");
      set => this.Q("q", (object) value);
    }

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }

    public long? TerminateAfter
    {
      get => this.Q<long?>("terminate_after");
      set => this.Q("terminate_after", (object) value);
    }

    public QueryContainer Query { get; set; }

    protected override HttpMethod HttpMethod => !this.Self.RequestParameters.ContainsQueryString("source") && !this.Self.RequestParameters.ContainsQueryString("q") && this.Self.Query != null && !this.Self.Query.IsConditionless() ? HttpMethod.POST : HttpMethod.GET;
  }
}
