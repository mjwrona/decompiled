// Decompiled with JetBrains decompiler
// Type: Nest.NodesHotThreadsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class NodesHotThreadsRequest : 
    PlainRequestBase<NodesHotThreadsRequestParameters>,
    INodesHotThreadsRequest,
    IRequest<NodesHotThreadsRequestParameters>,
    IRequest
  {
    protected override string ContentType => "text/plain";

    protected override sealed void RequestDefaults(NodesHotThreadsRequestParameters parameters) => parameters.CustomResponseBuilder = (CustomResponseBuilderBase) NodeHotThreadsResponseBuilder.Instance;

    protected INodesHotThreadsRequest Self => (INodesHotThreadsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesHotThreads;

    public NodesHotThreadsRequest()
    {
    }

    public NodesHotThreadsRequest(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    [IgnoreDataMember]
    NodeIds INodesHotThreadsRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    public bool? IgnoreIdleThreads
    {
      get => this.Q<bool?>("ignore_idle_threads");
      set => this.Q("ignore_idle_threads", (object) value);
    }

    public Time Interval
    {
      get => this.Q<Time>("interval");
      set => this.Q("interval", (object) value);
    }

    public long? Snapshots
    {
      get => this.Q<long?>("snapshots");
      set => this.Q("snapshots", (object) value);
    }

    public Elasticsearch.Net.Sort? Sort
    {
      get => this.Q<Elasticsearch.Net.Sort?>("sort");
      set => this.Q("sort", (object) value);
    }

    public Elasticsearch.Net.ThreadType? ThreadType
    {
      get => this.Q<Elasticsearch.Net.ThreadType?>("type");
      set => this.Q("type", (object) value);
    }

    public long? Threads
    {
      get => this.Q<long?>("threads");
      set => this.Q("threads", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
