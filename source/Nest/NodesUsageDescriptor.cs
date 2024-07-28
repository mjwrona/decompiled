// Decompiled with JetBrains decompiler
// Type: Nest.NodesUsageDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;

namespace Nest
{
  public class NodesUsageDescriptor : 
    RequestDescriptorBase<NodesUsageDescriptor, NodesUsageRequestParameters, INodesUsageRequest>,
    INodesUsageRequest,
    IRequest<NodesUsageRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesUsage;

    public NodesUsageDescriptor()
    {
    }

    public NodesUsageDescriptor(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    public NodesUsageDescriptor(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public NodesUsageDescriptor(NodeIds nodeId, Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId).Optional(nameof (metric), metric)))
    {
    }

    NodeIds INodesUsageRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    Metrics INodesUsageRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    public NodesUsageDescriptor NodeId(NodeIds nodeId) => this.Assign<NodeIds>(nodeId, (Action<INodesUsageRequest, NodeIds>) ((a, v) => a.RouteValues.Optional("node_id", (IUrlParameter) v)));

    public NodesUsageDescriptor Metric(Metrics metric) => this.Assign<Metrics>(metric, (Action<INodesUsageRequest, Metrics>) ((a, v) => a.RouteValues.Optional(nameof (metric), v)));

    public NodesUsageDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
