// Decompiled with JetBrains decompiler
// Type: Nest.ClusterAllocationExplainDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;
using System;

namespace Nest
{
  public class ClusterAllocationExplainDescriptor : 
    RequestDescriptorBase<ClusterAllocationExplainDescriptor, ClusterAllocationExplainRequestParameters, IClusterAllocationExplainRequest>,
    IClusterAllocationExplainRequest,
    IRequest<ClusterAllocationExplainRequestParameters>,
    IRequest
  {
    IndexName IClusterAllocationExplainRequest.Index { get; set; }

    bool? IClusterAllocationExplainRequest.Primary { get; set; }

    int? IClusterAllocationExplainRequest.Shard { get; set; }

    public ClusterAllocationExplainDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IClusterAllocationExplainRequest, IndexName>) ((a, v) => a.Index = v));

    public ClusterAllocationExplainDescriptor Index<TDocument>() => this.Assign<Type>(typeof (TDocument), (Action<IClusterAllocationExplainRequest, Type>) ((a, v) => a.Index = (IndexName) v));

    public ClusterAllocationExplainDescriptor Primary(bool? primary = true) => this.Assign<bool?>(primary, (Action<IClusterAllocationExplainRequest, bool?>) ((a, v) => a.Primary = v));

    public ClusterAllocationExplainDescriptor Shard(int? shard) => this.Assign<int?>(shard, (Action<IClusterAllocationExplainRequest, int?>) ((a, v) => a.Shard = v));

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterAllocationExplain;

    public ClusterAllocationExplainDescriptor IncludeDiskInfo(bool? includediskinfo = true) => this.Qs("include_disk_info", (object) includediskinfo);

    public ClusterAllocationExplainDescriptor IncludeYesDecisions(bool? includeyesdecisions = true) => this.Qs("include_yes_decisions", (object) includeyesdecisions);
  }
}
