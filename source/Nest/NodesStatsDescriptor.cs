// Decompiled with JetBrains decompiler
// Type: Nest.NodesStatsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class NodesStatsDescriptor : 
    RequestDescriptorBase<NodesStatsDescriptor, NodesStatsRequestParameters, INodesStatsRequest>,
    INodesStatsRequest,
    IRequest<NodesStatsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesStats;

    public NodesStatsDescriptor()
    {
    }

    public NodesStatsDescriptor(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    public NodesStatsDescriptor(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public NodesStatsDescriptor(NodeIds nodeId, Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId).Optional(nameof (metric), metric)))
    {
    }

    public NodesStatsDescriptor(Metrics metric, IndexMetrics indexMetric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric).Optional("index_metric", indexMetric)))
    {
    }

    public NodesStatsDescriptor(NodeIds nodeId, Metrics metric, IndexMetrics indexMetric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId).Optional(nameof (metric), metric).Optional("index_metric", indexMetric)))
    {
    }

    NodeIds INodesStatsRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    Metrics INodesStatsRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    IndexMetrics INodesStatsRequest.IndexMetric => this.Self.RouteValues.Get<IndexMetrics>("index_metric");

    public NodesStatsDescriptor NodeId(NodeIds nodeId) => this.Assign<NodeIds>(nodeId, (Action<INodesStatsRequest, NodeIds>) ((a, v) => a.RouteValues.Optional("node_id", (IUrlParameter) v)));

    public NodesStatsDescriptor Metric(Metrics metric) => this.Assign<Metrics>(metric, (Action<INodesStatsRequest, Metrics>) ((a, v) => a.RouteValues.Optional(nameof (metric), v)));

    public NodesStatsDescriptor IndexMetric(IndexMetrics indexMetric) => this.Assign<IndexMetrics>(indexMetric, (Action<INodesStatsRequest, IndexMetrics>) ((a, v) => a.RouteValues.Optional("index_metric", v)));

    public NodesStatsDescriptor CompletionFields(Nest.Fields completionfields) => this.Qs("completion_fields", (object) completionfields);

    public NodesStatsDescriptor CompletionFields<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs("completion_fields", fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public NodesStatsDescriptor FielddataFields(Nest.Fields fielddatafields) => this.Qs("fielddata_fields", (object) fielddatafields);

    public NodesStatsDescriptor FielddataFields<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs("fielddata_fields", fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public NodesStatsDescriptor Fields(Nest.Fields fields) => this.Qs(nameof (fields), (object) fields);

    public NodesStatsDescriptor Fields<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs(nameof (fields), fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public NodesStatsDescriptor Groups(bool? groups = true) => this.Qs(nameof (groups), (object) groups);

    public NodesStatsDescriptor IncludeSegmentFileSizes(bool? includesegmentfilesizes = true) => this.Qs("include_segment_file_sizes", (object) includesegmentfilesizes);

    public NodesStatsDescriptor IncludeUnloadedSegments(bool? includeunloadedsegments = true) => this.Qs("include_unloaded_segments", (object) includeunloadedsegments);

    public NodesStatsDescriptor Level(Elasticsearch.Net.Level? level) => this.Qs(nameof (level), (object) level);

    public NodesStatsDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public NodesStatsDescriptor Types(params string[] types) => this.Qs(nameof (types), (object) types);
  }
}
