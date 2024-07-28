// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskTemplateStep
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TaskTemplateStep : Step
  {
    [DataMember(Name = "Parameters", EmitDefaultValue = false)]
    private IDictionary<string, string> m_parameters;

    public TaskTemplateStep()
    {
    }

    private TaskTemplateStep(TaskTemplateStep templateToClone)
      : base((Step) templateToClone)
    {
      this.Reference = templateToClone.Reference?.Clone();
      IDictionary<string, string> parameters = templateToClone.m_parameters;
      if ((parameters != null ? (parameters.Count > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_parameters = (IDictionary<string, string>) new Dictionary<string, string>(templateToClone.m_parameters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public override StepType Type => StepType.TaskTemplate;

    public IDictionary<string, string> Parameters
    {
      get
      {
        if (this.m_parameters == null)
          this.m_parameters = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_parameters;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public TaskTemplateReference Reference { get; set; }

    public override Step Clone() => (Step) new TaskTemplateStep(this);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IDictionary<string, string> parameters = this.m_parameters;
      if ((parameters != null ? (parameters.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_parameters = (IDictionary<string, string>) null;
    }
  }
}
