// Decompiled with JetBrains decompiler
// Type: Nest.ClusterStatsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using System;

namespace Nest
{
  public class ClusterStatsDescriptor : 
    RequestDescriptorBase<ClusterStatsDescriptor, ClusterStatsRequestParameters, IClusterStatsRequest>,
    IClusterStatsRequest,
    IRequest<ClusterStatsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterStats;

    public ClusterStatsDescriptor()
    {
    }

    public ClusterStatsDescriptor(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    NodeIds IClusterStatsRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    public ClusterStatsDescriptor NodeId(NodeIds nodeId) => this.Assign<NodeIds>(nodeId, (Action<IClusterStatsRequest, NodeIds>) ((a, v) => a.RouteValues.Optional("node_id", (IUrlParameter) v)));

    public ClusterStatsDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public ClusterStatsDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
