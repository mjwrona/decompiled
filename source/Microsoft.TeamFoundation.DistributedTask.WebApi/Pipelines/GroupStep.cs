// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.GroupStep
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GroupStep : JobStep
  {
    [DataMember(Name = "Steps", EmitDefaultValue = false)]
    private IList<TaskStep> m_steps;
    [DataMember(Name = "Outputs", EmitDefaultValue = false)]
    private IDictionary<string, string> m_outputs;

    [JsonConstructor]
    public GroupStep()
    {
    }

    private GroupStep(GroupStep groupStepToClone)
      : base((JobStep) groupStepToClone)
    {
      IList<TaskStep> steps = groupStepToClone.m_steps;
      if ((steps != null ? (steps.Count > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (Step step in (IEnumerable<TaskStep>) groupStepToClone.m_steps)
          this.Steps.Add(step.Clone() as TaskStep);
      }
      IDictionary<string, string> outputs = groupStepToClone.m_outputs;
      if ((outputs != null ? (outputs.Count > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_outputs = (IDictionary<string, string>) new Dictionary<string, string>(groupStepToClone.m_outputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public override StepType Type => StepType.Group;

    public IList<TaskStep> Steps
    {
      get
      {
        if (this.m_steps == null)
          this.m_steps = (IList<TaskStep>) new List<TaskStep>();
        return this.m_steps;
      }
    }

    public IDictionary<string, string> Outputs
    {
      get
      {
        if (this.m_outputs == null)
          this.m_outputs = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_outputs;
      }
    }

    public override Step Clone() => (Step) new GroupStep(this);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IList<TaskStep> steps = this.m_steps;
      if ((steps != null ? (steps.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_steps = (IList<TaskStep>) null;
      IDictionary<string, string> outputs = this.m_outputs;
      if ((outputs != null ? (outputs.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_outputs = (IDictionary<string, string>) null;
    }
  }
}
