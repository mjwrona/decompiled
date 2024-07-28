// Decompiled with JetBrains decompiler
// Type: Nest.NodesStatsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class NodesStatsRequest : 
    PlainRequestBase<NodesStatsRequestParameters>,
    INodesStatsRequest,
    IRequest<NodesStatsRequestParameters>,
    IRequest
  {
    protected INodesStatsRequest Self => (INodesStatsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesStats;

    public NodesStatsRequest()
    {
    }

    public NodesStatsRequest(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    public NodesStatsRequest(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public NodesStatsRequest(NodeIds nodeId, Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId).Optional(nameof (metric), metric)))
    {
    }

    public NodesStatsRequest(Metrics metric, IndexMetrics indexMetric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric).Optional("index_metric", indexMetric)))
    {
    }

    public NodesStatsRequest(NodeIds nodeId, Metrics metric, IndexMetrics indexMetric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId).Optional(nameof (metric), metric).Optional("index_metric", indexMetric)))
    {
    }

    [IgnoreDataMember]
    NodeIds INodesStatsRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    [IgnoreDataMember]
    Metrics INodesStatsRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    [IgnoreDataMember]
    IndexMetrics INodesStatsRequest.IndexMetric => this.Self.RouteValues.Get<IndexMetrics>("index_metric");

    public Fields CompletionFields
    {
      get => this.Q<Fields>("completion_fields");
      set => this.Q("completion_fields", (object) value);
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

    public bool? Groups
    {
      get => this.Q<bool?>("groups");
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

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public string[] Types
    {
      get => this.Q<string[]>("types");
      set => this.Q("types", (object) value);
    }
  }
}
