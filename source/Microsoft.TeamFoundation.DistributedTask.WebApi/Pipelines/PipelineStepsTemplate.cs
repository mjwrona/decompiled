// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineStepsTemplate
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineStepsTemplate
  {
    [DataMember(Name = "Steps", EmitDefaultValue = false)]
    private List<Step> m_steps;
    [DataMember(Name = "Errors", EmitDefaultValue = false)]
    private List<PipelineValidationError> m_errors;

    public IList<Step> Steps
    {
      get
      {
        if (this.m_steps == null)
          this.m_steps = new List<Step>();
        return (IList<Step>) this.m_steps;
      }
    }

    public IList<PipelineValidationError> Errors
    {
      get
      {
        if (this.m_errors == null)
          this.m_errors = new List<PipelineValidationError>();
        return (IList<PipelineValidationError>) this.m_errors;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<Step> steps = this.m_steps;
      // ISSUE: explicit non-virtual call
      if ((steps != null ? (__nonvirtual (steps.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_steps = (List<Step>) null;
      List<PipelineValidationError> errors = this.m_errors;
      // ISSUE: explicit non-virtual call
      if ((errors != null ? (__nonvirtual (errors.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_errors = (List<PipelineValidationError>) null;
    }
  }
}
