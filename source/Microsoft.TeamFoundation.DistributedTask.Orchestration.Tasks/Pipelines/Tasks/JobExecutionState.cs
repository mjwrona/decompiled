// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.JobExecutionState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public class JobExecutionState
  {
    [DataMember(Name = "Tasks", EmitDefaultValue = false)]
    private List<TaskExecutionState> m_tasks;

    [JsonConstructor]
    public JobExecutionState()
    {
      this.Attempt = 1;
      this.CancelTimeoutInMinutes = 5;
      this.CheckRerunAttempt = 1;
    }

    public JobExecutionState(JobInstance job)
      : this()
    {
      this.Attempt = job.Attempt;
      this.Name = job.Name;
      this.CheckRerunAttempt = job.CheckRerunAttempt;
      Job definition = job.Definition;
      if (definition == null)
        return;
      this.Target = definition.Target.TrimForExecution();
      this.ContinueOnError = definition.ContinueOnError;
      this.TimeoutInMinutes = definition.TimeoutInMinutes;
      this.CancelTimeoutInMinutes = definition.CancelTimeoutInMinutes;
    }

    [DataMember(EmitDefaultValue = false)]
    public PhaseTarget Target { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int Attempt { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int CheckRerunAttempt { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int TimeoutInMinutes { get; set; }

    [DefaultValue(5)]
    [DataMember(EmitDefaultValue = false)]
    public int CancelTimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int RetryCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; set; }

    public IList<TaskExecutionState> Tasks
    {
      get
      {
        if (this.m_tasks == null)
          this.m_tasks = new List<TaskExecutionState>();
        return (IList<TaskExecutionState>) this.m_tasks;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public JobError Error { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool DeliveryFailed { get; set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<TaskExecutionState> tasks = this.m_tasks;
      // ISSUE: explicit non-virtual call
      if ((tasks != null ? (__nonvirtual (tasks.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_tasks = (List<TaskExecutionState>) null;
    }

    internal void CopyFrom(JobExecutionState source)
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
