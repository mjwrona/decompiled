// Decompiled with JetBrains decompiler
// Type: Nest.NodesHotThreadsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;

namespace Nest
{
  public class NodesHotThreadsDescriptor : 
    RequestDescriptorBase<NodesHotThreadsDescriptor, NodesHotThreadsRequestParameters, INodesHotThreadsRequest>,
    INodesHotThreadsRequest,
    IRequest<NodesHotThreadsRequestParameters>,
    IRequest
  {
    protected override string ContentType => "text/plain";

    protected override sealed void RequestDefaults(NodesHotThreadsRequestParameters parameters) => parameters.CustomResponseBuilder = (CustomResponseBuilderBase) NodeHotThreadsResponseBuilder.Instance;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesHotThreads;

    public NodesHotThreadsDescriptor()
    {
    }

    public NodesHotThreadsDescriptor(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    NodeIds INodesHotThreadsRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    public NodesHotThreadsDescriptor NodeId(NodeIds nodeId) => this.Assign<NodeIds>(nodeId, (Action<INodesHotThreadsRequest, NodeIds>) ((a, v) => a.RouteValues.Optional("node_id", (IUrlParameter) v)));

    public NodesHotThreadsDescriptor IgnoreIdleThreads(bool? ignoreidlethreads = true) => this.Qs("ignore_idle_threads", (object) ignoreidlethreads);

    public NodesHotThreadsDescriptor Interval(Time interval) => this.Qs(nameof (interval), (object) interval);

    public NodesHotThreadsDescriptor Snapshots(long? snapshots) => this.Qs(nameof (snapshots), (object) snapshots);

    public NodesHotThreadsDescriptor Sort(Elasticsearch.Net.Sort? sort) => this.Qs(nameof (sort), (object) sort);

    public NodesHotThreadsDescriptor ThreadType(Elasticsearch.Net.ThreadType? threadtype) => this.Qs("type", (object) threadtype);

    public NodesHotThreadsDescriptor Threads(long? threads) => this.Qs(nameof (threads), (object) threads);

    public NodesHotThreadsDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
