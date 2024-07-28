// Decompiled with JetBrains decompiler
// Type: Nest.ClusterPendingTasksDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;

namespace Nest
{
  public class ClusterPendingTasksDescriptor : 
    RequestDescriptorBase<ClusterPendingTasksDescriptor, ClusterPendingTasksRequestParameters, IClusterPendingTasksRequest>,
    IClusterPendingTasksRequest,
    IRequest<ClusterPendingTasksRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterPendingTasks;

    public ClusterPendingTasksDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public ClusterPendingTasksDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
