// Decompiled with JetBrains decompiler
// Type: Nest.NodesInfoDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;

namespace Nest
{
  public class NodesInfoDescriptor : 
    RequestDescriptorBase<NodesInfoDescriptor, NodesInfoRequestParameters, INodesInfoRequest>,
    INodesInfoRequest,
    IRequest<NodesInfoRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesInfo;

    public NodesInfoDescriptor()
    {
    }

    public NodesInfoDescriptor(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    public NodesInfoDescriptor(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public NodesInfoDescriptor(NodeIds nodeId, Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId).Optional(nameof (metric), metric)))
    {
    }

    NodeIds INodesInfoRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    Metrics INodesInfoRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    public NodesInfoDescriptor NodeId(NodeIds nodeId) => this.Assign<NodeIds>(nodeId, (Action<INodesInfoRequest, NodeIds>) ((a, v) => a.RouteValues.Optional("node_id", (IUrlParameter) v)));

    public NodesInfoDescriptor Metric(Metrics metric) => this.Assign<Metrics>(metric, (Action<INodesInfoRequest, Metrics>) ((a, v) => a.RouteValues.Optional(nameof (metric), v)));

    public NodesInfoDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public NodesInfoDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
