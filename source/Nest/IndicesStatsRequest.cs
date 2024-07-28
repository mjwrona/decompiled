// Decompiled with JetBrains decompiler
// Type: Nest.IndicesStatsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class IndicesStatsRequest : 
    PlainRequestBase<IndicesStatsRequestParameters>,
    IIndicesStatsRequest,
    IRequest<IndicesStatsRequestParameters>,
    IRequest
  {
    protected IIndicesStatsRequest Self => (IIndicesStatsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesStats;

    public IndicesStatsRequest()
    {
    }

    public IndicesStatsRequest(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public IndicesStatsRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    public IndicesStatsRequest(Indices index, Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index).Optional(nameof (metric), metric)))
    {
    }

    [IgnoreDataMember]
    Metrics IIndicesStatsRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    [IgnoreDataMember]
    Indices IIndicesStatsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public Fields CompletionFields
    {
      get => this.Q<Fields>("completion_fields");
      set => this.Q("completion_fields", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public Fields FielddataFields
    {
      get => this.Q<Fields>("fielddata_fields");
      set => this.Q("fielddata_fields", (object) value);
    }

    public Fields Fields
    {
      get => this.Q<Fields>("fields");
      set => this.Q("fields", (object) value);
    }

    public bool? ForbidClosedIndices
    {
      get => this.Q<bool?>("forbid_closed_indices");
      set => this.Q("forbid_closed_indices", (object) value);
    }

    public string[] Groups
    {
      get => this.Q<string[]>("groups");
      set => this.Q("groups", (object) value);
    }

    public bool? IncludeSegmentFileSizes
    {
      get => this.Q<bool?>("include_segment_file_sizes");
      set => this.Q("include_segment_file_sizes", (object) value);
    }

    public bool? IncludeUnloadedSegments
    {
      get => this.Q<bool?>("include_unloaded_segments");
      set => this.Q("include_unloaded_segments", (object) value);
    }

    public Elasticsearch.Net.Level? Level
    {
      get => this.Q<Elasticsearch.Net.Level?>("level");
      set => this.Q("level", (object) value);
    }
  }
}
