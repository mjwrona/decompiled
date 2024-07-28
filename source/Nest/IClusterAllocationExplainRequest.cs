// Decompiled with JetBrains decompiler
// Type: Nest.IClusterAllocationExplainRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("cluster.allocation_explain.json")]
  [ReadAs(typeof (ClusterAllocationExplainRequest))]
  [InterfaceDataContract]
  public interface IClusterAllocationExplainRequest : 
    IRequest<ClusterAllocationExplainRequestParameters>,
    IRequest
  {
    [DataMember(Name = "index")]
    IndexName Index { get; set; }

    [DataMember(Name = "primary")]
    bool? Primary { get; set; }

    [DataMember(Name = "shard")]
    int? Shard { get; set; }
  }
}
