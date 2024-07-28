// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunGraphInput`2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public abstract class RunGraphInput<TGraph, TNode>
    where TGraph : IGraph<TNode>
    where TNode : IGraphNode
  {
    protected RunGraphInput() => this.PlanVersion = 1;

    [DataMember(EmitDefaultValue = false)]
    public Guid ScopeId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PlanId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int PlanVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TGraph Pipeline { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int ActivityDispatcherShardsCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineActivityShardKey ShardKey { get; set; }
  }
}
