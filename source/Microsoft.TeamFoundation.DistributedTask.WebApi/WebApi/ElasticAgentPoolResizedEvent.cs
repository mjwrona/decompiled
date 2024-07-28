// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ElasticAgentPoolResizedEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-distributed-task.elastic-pool-resized-event")]
  public class ElasticAgentPoolResizedEvent
  {
    [DebuggerStepThrough]
    public ElasticAgentPoolResizedEvent()
    {
    }

    [DebuggerStepThrough]
    public ElasticAgentPoolResizedEvent(
      TaskAgentPool pool,
      ElasticPool elasticPool,
      int previousSize,
      int newSize)
    {
      ArgumentUtility.CheckForNull<TaskAgentPool>(pool, nameof (pool));
      ArgumentUtility.CheckForNull<ElasticPool>(elasticPool, nameof (elasticPool));
      this.PoolId = pool.Id;
      this.PoolName = pool.Name;
      this.ResourceId = elasticPool.AzureId;
      this.PreviousSize = previousSize;
      this.NewSize = newSize;
    }

    [DataMember]
    public int PoolId { get; set; }

    [DataMember]
    public string PoolName { get; set; }

    [DataMember]
    public string ResourceId { get; set; }

    [DataMember]
    public int PreviousSize { get; set; }

    [DataMember]
    public int NewSize { get; set; }
  }
}
