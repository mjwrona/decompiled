// Decompiled with JetBrains decompiler
// Type: Nest.ClusterStatsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ClusterStatsRequest : 
    PlainRequestBase<ClusterStatsRequestParameters>,
    IClusterStatsRequest,
    IRequest<ClusterStatsRequestParameters>,
    IRequest
  {
    protected IClusterStatsRequest Self => (IClusterStatsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterStats;

    public ClusterStatsRequest()
    {
    }

    public ClusterStatsRequest(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    [IgnoreDataMember]
    NodeIds IClusterStatsRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

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
