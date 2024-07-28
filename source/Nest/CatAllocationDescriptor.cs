// Decompiled with JetBrains decompiler
// Type: Nest.CatAllocationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatAllocationDescriptor : 
    RequestDescriptorBase<CatAllocationDescriptor, CatAllocationRequestParameters, ICatAllocationRequest>,
    ICatAllocationRequest,
    IRequest<CatAllocationRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatAllocation;

    public CatAllocationDescriptor()
    {
    }

    public CatAllocationDescriptor(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    NodeIds ICatAllocationRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    public CatAllocationDescriptor NodeId(NodeIds nodeId) => this.Assign<NodeIds>(nodeId, (Action<ICatAllocationRequest, NodeIds>) ((a, v) => a.RouteValues.Optional("node_id", (IUrlParameter) v)));

    public CatAllocationDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatAllocationDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatAllocationDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatAllocationDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatAllocationDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatAllocationDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatAllocationDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatAllocationDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
