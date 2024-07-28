// Decompiled with JetBrains decompiler
// Type: Nest.NodesUsageRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class NodesUsageRequest : 
    PlainRequestBase<NodesUsageRequestParameters>,
    INodesUsageRequest,
    IRequest<NodesUsageRequestParameters>,
    IRequest
  {
    protected INodesUsageRequest Self => (INodesUsageRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesUsage;

    public NodesUsageRequest()
    {
    }

    public NodesUsageRequest(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    public NodesUsageRequest(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public NodesUsageRequest(NodeIds nodeId, Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId).Optional(nameof (metric), metric)))
    {
    }

    [IgnoreDataMember]
    NodeIds INodesUsageRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    [IgnoreDataMember]
    Metrics INodesUsageRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
