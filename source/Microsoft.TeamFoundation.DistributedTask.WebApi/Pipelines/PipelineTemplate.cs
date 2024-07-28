// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineTemplate
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
  public class PipelineTemplate
  {
    [DataMember(Name = "Stages", EmitDefaultValue = false)]
    private List<Stage> m_stages;
    [DataMember(Name = "Errors", EmitDefaultValue = false)]
    private List<PipelineValidationError> m_errors;
    [DataMember(Name = "Triggers", EmitDefaultValue = false)]
    private List<PipelineTrigger> m_triggers;
    [DataMember(Name = "Resources", EmitDefaultValue = false)]
    private PipelineResources m_resources;
    [DataMember(Name = "Parameters", EmitDefaultValue = false)]
    private List<TemplateParameter> m_parameters;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private List<IVariable> m_variables;
    [DataMember(Name = "Schedules", EmitDefaultValue = false)]
    private List<PipelineSchedule> m_schedules;

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool AppendCommitMessageToRunName { get; set; } = true;

    [DataMember(EmitDefaultValue = false)]
    public PipelineResources Resources
    {
      get
      {
        if (this.m_resources == null)
          this.m_resources = new PipelineResources();
        return this.m_resources;
      }
    }

    public IList<TemplateParameter> Parameters
    {
      get
      {
        if (this.m_parameters == null)
          this.m_parameters = new List<TemplateParameter>();
        return (IList<TemplateParameter>) this.m_parameters;
      }
    }

    public IList<IVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new List<IVariable>();
        return (IList<IVariable>) this.m_variables;
      }
    }

    public IList<Stage> Stages
    {
      get
      {
        if (this.m_stages == null)
          this.m_stages = new List<Stage>();
        return (IList<Stage>) this.m_stages;
      }
    }

    public IList<PipelineTrigger> Triggers
    {
      get
      {
        if (this.m_triggers == null)
          this.m_triggers = new List<PipelineTrigger>();
        return (IList<PipelineTrigger>) this.m_triggers;
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

    public IList<PipelineSchedule> Schedules
    {
      get
      {
        if (this.m_schedules == null)
          this.m_schedules = new List<PipelineSchedule>();
        return (IList<PipelineSchedule>) this.m_schedules;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string InitializationLog { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ExpandedYaml { get; set; }

    public void CheckErrors()
    {
      List<PipelineValidationError> errors = this.m_errors;
      // ISSUE: explicit non-virtual call
      if ((errors != null ? (__nonvirtual (errors.Count) > 0 ? 1 : 0) : 0) != 0)
        throw new PipelineValidationException((IEnumerable<PipelineValidationError>) this.m_errors);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<Stage> stages = this.m_stages;
      // ISSUE: explicit non-virtual call
      if ((stages != null ? (__nonvirtual (stages.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_stages = (List<Stage>) null;
      List<PipelineValidationError> errors = this.m_errors;
      // ISSUE: explicit non-virtual call
      if ((errors != null ? (__nonvirtual (errors.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_errors = (List<PipelineValidationError>) null;
      List<PipelineTrigger> triggers = this.m_triggers;
      // ISSUE: explicit non-virtual call
      if ((triggers != null ? (__nonvirtual (triggers.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_triggers = (List<PipelineTrigger>) null;
      PipelineResources resources = this.m_resources;
      if ((resources != null ? (resources.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_resources = (PipelineResources) null;
      List<TemplateParameter> parameters = this.m_parameters;
      // ISSUE: explicit non-virtual call
      if ((parameters != null ? (__nonvirtual (parameters.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_parameters = (List<TemplateParameter>) null;
      List<IVariable> variables = this.m_variables;
      // ISSUE: explicit non-virtual call
      if ((variables != null ? (__nonvirtual (variables.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_variables = (List<IVariable>) null;
      List<PipelineSchedule> schedules = this.m_schedules;
      // ISSUE: explicit non-virtual call
      if ((schedules != null ? (__nonvirtual (schedules.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_schedules = (List<PipelineSchedule>) null;
    }
  }
}
