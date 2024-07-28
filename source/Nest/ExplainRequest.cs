// Decompiled with JetBrains decompiler
// Type: Nest.ExplainRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ExplainRequest : 
    PlainRequestBase<ExplainRequestParameters>,
    IExplainRequest,
    IRequest<ExplainRequestParameters>,
    IRequest
  {
    protected IExplainRequest Self => (IExplainRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceExplain;

    public ExplainRequest(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected ExplainRequest()
    {
    }

    [IgnoreDataMember]
    IndexName IExplainRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    [IgnoreDataMember]
    Id IExplainRequest.Id => this.Self.RouteValues.Get<Id>("id");

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

    public bool? Lenient
    {
      get => this.Q<bool?>("lenient");
      set => this.Q("lenient", (object) value);
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

    public bool? SourceEnabled
    {
      get => this.Q<bool?>("_source");
      set => this.Q("_source", (object) value);
    }

    public Fields SourceExcludes
    {
      get => this.Q<Fields>("_source_excludes");
      set => this.Q("_source_excludes", (object) value);
    }

    public Fields SourceIncludes
    {
      get => this.Q<Fields>("_source_includes");
      set => this.Q("_source_includes", (object) value);
    }

    public QueryContainer Query { get; set; }

    public Fields StoredFields { get; set; }
  }
}
