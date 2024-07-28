// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.PipelineActivityShardKey
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public class PipelineActivityShardKey : IActivityShardKey
  {
    [DataMember]
    public Guid ScopeId { get; set; }

    [DataMember]
    public int DefinitionId { get; set; }

    [DataMember]
    public int OwnerId { get; set; }

    public string GetGroupKey() => string.Format("{0}/{1}", (object) this.ScopeId, (object) this.DefinitionId);

    public int GetShardKey() => this.OwnerId;
  }
}
