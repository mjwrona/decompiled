// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunPhaseInput
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public class RunPhaseInput
  {
    [DataMember(Name = "Dependencies", EmitDefaultValue = false)]
    private IList<PhaseExecutionState> m_dependencies;
    [DataMember(Name = "DependsOn", EmitDefaultValue = false)]
    private IDictionary<string, PhaseExecutionState> m_dependsOn;

    public RunPhaseInput() => this.StageAttempt = 1;

    [DataMember(EmitDefaultValue = false)]
    public Guid ScopeId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PlanId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int PlanVersion { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int StageAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StageName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PhaseExecutionState Phase { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int ActivityDispatcherShardsCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineActivityShardKey ShardKey { get; set; }

    public IDictionary<string, PhaseExecutionState> DependsOn
    {
      get
      {
        if (this.m_dependsOn == null)
          this.m_dependsOn = (IDictionary<string, PhaseExecutionState>) new Dictionary<string, PhaseExecutionState>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_dependsOn;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IDictionary<string, PhaseExecutionState> dependsOn = this.m_dependsOn;
      if ((dependsOn != null ? (dependsOn.Count > 0 ? 1 : 0) : 0) != 0)
      {
        this.m_dependencies = (IList<PhaseExecutionState>) new List<PhaseExecutionState>((IEnumerable<PhaseExecutionState>) this.m_dependsOn.Values);
        this.m_dependsOn = (IDictionary<string, PhaseExecutionState>) null;
      }
      else
      {
        this.m_dependsOn = (IDictionary<string, PhaseExecutionState>) null;
        this.m_dependencies = (IList<PhaseExecutionState>) null;
      }
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      IList<PhaseExecutionState> dependencies = this.m_dependencies;
      if ((dependencies != null ? (dependencies.Count > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_dependsOn = (IDictionary<string, PhaseExecutionState>) this.m_dependencies.ToDictionary<PhaseExecutionState, string, PhaseExecutionState>((Func<PhaseExecutionState, string>) (x => x.Name), (Func<PhaseExecutionState, PhaseExecutionState>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_dependencies = (IList<PhaseExecutionState>) null;
    }
  }
}
