// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.TaskExecutionState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public class TaskExecutionState
  {
    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Condition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Enabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int TimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int RetryCountOnTaskFailure { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskError Error { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool DeliveryFailed { get; set; }

    internal void CopyFrom(TaskExecutionState source)
    {
      this.Error = source.Error;
      this.StartTime = source.StartTime;
      this.FinishTime = source.FinishTime;
      this.State = source.State;
      this.Result = source.Result;
      this.DeliveryFailed = source.DeliveryFailed;
    }
  }
}
