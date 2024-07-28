// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.PhaseConditionContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  [JsonConverter(typeof (SimpleJsonConverter<PhaseConditionContext>))]
  public class PhaseConditionContext
  {
    [DataMember(Name = "Dependencies", EmitDefaultValue = false)]
    private IDictionary<string, PhaseExecutionState> m_dependencies;

    public PhaseConditionContext()
    {
      this.StageAttempt = 1;
      this.PhaseAttempt = 1;
    }

    public PhaseConditionContext(
      PipelineState state,
      string stageName,
      string phaseName,
      Guid phaseId)
    {
      this.State = state;
      this.StageName = stageName;
      this.PhaseName = phaseName;
      this.PhaseId = phaseId;
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid ScopeId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PlanId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PhaseId { get; internal set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int StageAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StageName { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int PhaseAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PhaseName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineState State { get; set; }

    public IDictionary<string, PhaseExecutionState> Dependencies
    {
      get
      {
        if (this.m_dependencies == null)
          this.m_dependencies = (IDictionary<string, PhaseExecutionState>) new Dictionary<string, PhaseExecutionState>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_dependencies;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IDictionary<string, PhaseExecutionState> dependencies = this.m_dependencies;
      if ((dependencies != null ? (dependencies.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_dependencies = (IDictionary<string, PhaseExecutionState>) null;
    }
  }
}
