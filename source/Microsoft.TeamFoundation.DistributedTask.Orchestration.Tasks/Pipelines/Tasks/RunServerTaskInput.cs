// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunServerTaskInput
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public sealed class RunServerTaskInput
  {
    public RunServerTaskInput()
    {
      this.StageAttempt = 1;
      this.PhaseAttempt = 1;
      this.JobAttempt = 1;
      this.CheckRerunAttempt = 1;
    }

    [DataMember]
    public Guid ScopeId { get; set; }

    [DataMember]
    public Guid PlanId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int PlanVersion { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int StageAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StageName { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int PhaseAttempt { get; set; }

    [DataMember]
    public string PhaseName { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int JobAttempt { get; set; }

    [DataMember]
    public string JobName { get; set; }

    [DataMember]
    public TaskExecutionState Task { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int CheckRerunAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int ActivityDispatcherShardsCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineActivityShardKey ShardKey { get; set; }
  }
}
