// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.StageExecutionState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  [JsonConverter(typeof (SimpleJsonConverter<StageExecutionState>))]
  public class StageExecutionState : IGraph<PhaseExecutionState>, IGraphNode
  {
    [DataMember(Name = "Phases", EmitDefaultValue = false)]
    private List<PhaseExecutionState> m_phases;
    [DataMember(Name = "Dependencies", EmitDefaultValue = false)]
    private GraphDependencies m_dependencies;

    [JsonConstructor]
    public StageExecutionState() => this.Attempt = 1;

    public StageExecutionState(Stage stage, bool includePhases = true)
    {
      this.Attempt = 1;
      if (stage == null)
        return;
      this.Name = stage.Name;
      this.Condition = stage.Condition;
      this.Skip = stage.Skip;
      if (stage.DependsOn.Count > 0)
      {
        this.m_dependencies = new GraphDependencies();
        this.m_dependencies.Unsatisfied.UnionWith((IEnumerable<string>) stage.DependsOn);
      }
      if (!includePhases)
        return;
      this.Phases.AddRange<PhaseExecutionState, IList<PhaseExecutionState>>(stage.Phases.Select<PhaseNode, PhaseExecutionState>((Func<PhaseNode, PhaseExecutionState>) (x => new PhaseExecutionState(x))));
    }

    public StageExecutionState(StageAttempt attempt)
      : this(attempt.Stage.Definition, false)
    {
      StageInstance stage = attempt.Stage;
      this.Attempt = stage.Attempt;
      this.Name = stage.Name;
      this.State = stage.State;
      this.Result = stage.Result;
      this.Phases.AddRange<PhaseExecutionState, IList<PhaseExecutionState>>(attempt.Phases.Select<PhaseAttempt, PhaseExecutionState>((Func<PhaseAttempt, PhaseExecutionState>) (x => new PhaseExecutionState(x))));
    }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int Attempt { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Condition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember]
    public PipelineState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; set; }

    [DataMember]
    public bool Skip { get; set; }

    public GraphDependencies Dependencies
    {
      get
      {
        if (this.m_dependencies == null)
          this.m_dependencies = new GraphDependencies();
        return this.m_dependencies;
      }
    }

    public IList<PhaseExecutionState> Phases
    {
      get
      {
        if (this.m_phases == null)
          this.m_phases = new List<PhaseExecutionState>();
        return (IList<PhaseExecutionState>) this.m_phases;
      }
    }

    public void CopyFrom(StageExecutionState source)
    {
      this.Attempt = source.Attempt;
      this.StartTime = source.StartTime;
      this.FinishTime = source.FinishTime;
      this.State = source.State;
      this.Result = source.Result;
      this.Skip = source.Skip;
      this.m_dependencies = source.m_dependencies;
      this.Phases.Clear();
      this.Phases.AddRange<PhaseExecutionState, IList<PhaseExecutionState>>((IEnumerable<PhaseExecutionState>) source.Phases);
    }

    public StageInstance ToInstance()
    {
      StageInstance instance = new StageInstance();
      instance.Attempt = this.Attempt;
      instance.FinishTime = this.FinishTime;
      instance.Name = this.Name;
      instance.StartTime = this.StartTime;
      instance.Result = new TaskResult?(this.Result.GetValueOrDefault());
      return instance;
    }

    public PhaseExecutionState Trim(PhaseExecutionState node) => new PhaseExecutionState()
    {
      Attempt = node.Attempt,
      FinishTime = node.FinishTime,
      Name = node.Name,
      Result = node.Result,
      StartTime = node.StartTime,
      State = node.State
    };

    IList<PhaseExecutionState> IGraph<PhaseExecutionState>.Nodes => this.Phases;

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<PhaseExecutionState> phases = this.m_phases;
      // ISSUE: explicit non-virtual call
      if ((phases != null ? (__nonvirtual (phases.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_phases = (List<PhaseExecutionState>) null;
      if (this.m_dependencies == null || this.m_dependencies.Satisfied.Count != 0 || this.m_dependencies.Unsatisfied.Count != 0)
        return;
      this.m_dependencies = (GraphDependencies) null;
    }
  }
}
