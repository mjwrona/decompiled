// Decompiled with JetBrains decompiler
// Type: Nest.ClusterAllocationExplainRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;

namespace Nest
{
  public class ClusterAllocationExplainRequest : 
    PlainRequestBase<ClusterAllocationExplainRequestParameters>,
    IClusterAllocationExplainRequest,
    IRequest<ClusterAllocationExplainRequestParameters>,
    IRequest
  {
    public IndexName Index { get; set; }

    public bool? Primary { get; set; }

    public int? Shard { get; set; }

    protected IClusterAllocationExplainRequest Self => (IClusterAllocationExplainRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterAllocationExplain;

    public bool? IncludeDiskInfo
    {
      get => this.Q<bool?>("include_disk_info");
      set => this.Q("include_disk_info", (object) value);
    }

    public bool? IncludeYesDecisions
    {
      get => this.Q<bool?>("include_yes_decisions");
      set => this.Q("include_yes_decisions", (object) value);
    }
  }
}
