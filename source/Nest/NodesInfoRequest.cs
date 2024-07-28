// Decompiled with JetBrains decompiler
// Type: Nest.NodesInfoRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class NodesInfoRequest : 
    PlainRequestBase<NodesInfoRequestParameters>,
    INodesInfoRequest,
    IRequest<NodesInfoRequestParameters>,
    IRequest
  {
    protected INodesInfoRequest Self => (INodesInfoRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesInfo;

    public NodesInfoRequest()
    {
    }

    public NodesInfoRequest(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    public NodesInfoRequest(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public NodesInfoRequest(NodeIds nodeId, Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId).Optional(nameof (metric), metric)))
    {
    }

    [IgnoreDataMember]
    NodeIds INodesInfoRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    [IgnoreDataMember]
    Metrics INodesInfoRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    public bool? FlatSettings
    {
      get => this.Q<bool?>("flat_settings");
      set => this.Q("flat_settings", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
