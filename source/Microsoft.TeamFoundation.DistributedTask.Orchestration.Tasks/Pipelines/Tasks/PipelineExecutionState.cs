// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.PipelineExecutionState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public class PipelineExecutionState : IGraph<StageExecutionState>
  {
    [DataMember(Name = "Stages", EmitDefaultValue = false)]
    private List<StageExecutionState> m_stages;
    [DataMember(Name = "Phases", EmitDefaultValue = false)]
    private List<PhaseExecutionState> m_phases;

    [JsonConstructor]
    public PipelineExecutionState()
    {
    }

    public PipelineExecutionState(PipelineProcess process) => this.m_stages = process.Stages.Select<Stage, StageExecutionState>((Func<Stage, StageExecutionState>) (x => new StageExecutionState(x))).ToList<StageExecutionState>();

    public PipelineExecutionState(IList<StageAttempt> attempts) => this.m_stages = attempts.Select<StageAttempt, StageExecutionState>((Func<StageAttempt, StageExecutionState>) (x => new StageExecutionState(x))).ToList<StageExecutionState>();

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; set; }

    public IList<StageExecutionState> Stages
    {
      get
      {
        if (this.m_stages == null)
          this.m_stages = new List<StageExecutionState>();
        return (IList<StageExecutionState>) this.m_stages;
      }
    }

    IList<StageExecutionState> IGraph<StageExecutionState>.Nodes => this.Stages;

    public StageExecutionState Trim(StageExecutionState node) => new StageExecutionState()
    {
      Attempt = node.Attempt,
      FinishTime = node.FinishTime,
      Name = node.Name,
      Result = node.Result,
      StartTime = node.StartTime,
      State = node.State
    };

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      List<StageExecutionState> stages = this.m_stages;
      // ISSUE: explicit non-virtual call
      if ((stages != null ? __nonvirtual (stages.Count) : 0) != 0)
        return;
      List<PhaseExecutionState> phases = this.m_phases;
      // ISSUE: explicit non-virtual call
      if ((phases != null ? (__nonvirtual (phases.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      StageExecutionState stageExecutionState = new StageExecutionState()
      {
        Name = PipelineConstants.DefaultJobName
      };
      stageExecutionState.Phases.AddRange<PhaseExecutionState, IList<PhaseExecutionState>>((IEnumerable<PhaseExecutionState>) this.m_phases);
      this.m_stages = new List<StageExecutionState>();
      this.m_stages.Add(stageExecutionState);
      this.m_phases = (List<PhaseExecutionState>) null;
    }
  }
}
