// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Job
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class Job
  {
    [DataMember(Name = "Demands", EmitDefaultValue = false)]
    private List<Demand> m_demands;
    [DataMember(Name = "Steps", EmitDefaultValue = false)]
    private List<JobStep> m_steps;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private List<IVariable> m_variables;
    [DataMember(Name = "SidecarContainers", EmitDefaultValue = false)]
    private IDictionary<string, string> m_sidecarContainers;

    [JsonConstructor]
    public Job()
    {
    }

    private Job(Job jobToCopy)
    {
      this.Id = jobToCopy.Id;
      this.Name = jobToCopy.Name;
      this.DisplayName = jobToCopy.DisplayName;
      this.Container = jobToCopy.Container;
      this.ContinueOnError = jobToCopy.ContinueOnError;
      this.TimeoutInMinutes = jobToCopy.TimeoutInMinutes;
      this.CancelTimeoutInMinutes = jobToCopy.CancelTimeoutInMinutes;
      this.Workspace = jobToCopy.Workspace?.Clone();
      this.Target = jobToCopy.Target?.Clone();
      if (jobToCopy.m_demands != null && jobToCopy.m_demands.Count > 0)
        this.m_demands = new List<Demand>(jobToCopy.m_demands.Select<Demand, Demand>((Func<Demand, Demand>) (x => x.Clone())));
      if (jobToCopy.m_steps != null && jobToCopy.m_steps.Count > 0)
        this.m_steps = new List<JobStep>(jobToCopy.m_steps.Select<JobStep, JobStep>((Func<JobStep, JobStep>) (x => x.Clone() as JobStep)));
      if (jobToCopy.m_variables != null && jobToCopy.m_variables.Count > 0)
        this.m_variables = new List<IVariable>((IEnumerable<IVariable>) jobToCopy.m_variables);
      if (jobToCopy.m_sidecarContainers == null || jobToCopy.m_sidecarContainers.Count <= 0)
        return;
      this.m_sidecarContainers = (IDictionary<string, string>) new Dictionary<string, string>(jobToCopy.m_sidecarContainers, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Container { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int TimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int CancelTimeoutInMinutes { get; set; }

    public IList<Demand> Demands
    {
      get
      {
        if (this.m_demands == null)
          this.m_demands = new List<Demand>();
        return (IList<Demand>) this.m_demands;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ExecuteAs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkspaceOptions Workspace { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PhaseTarget Target { get; set; }

    public IList<JobStep> Steps
    {
      get
      {
        if (this.m_steps == null)
          this.m_steps = new List<JobStep>();
        return (IList<JobStep>) this.m_steps;
      }
    }

    public IDictionary<string, string> SidecarContainers
    {
      get
      {
        if (this.m_sidecarContainers == null)
          this.m_sidecarContainers = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_sidecarContainers;
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

    public Job Clone() => new Job(this);

    public CreateTaskResult CreateTask(JobExecutionContext context, string taskName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(taskName, nameof (taskName));
      TaskDefinition definition = (TaskDefinition) null;
      if (this.Steps.SingleOrDefault<JobStep>((Func<JobStep, bool>) (x => taskName.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))?.Clone() is TaskStep task)
      {
        definition = context.TaskStore.ResolveTask(task.Reference.Id, task.Reference.Version);
        foreach (TaskInputDefinition taskInputDefinition in definition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x != null)))
        {
          string key = taskInputDefinition.Name?.Trim() ?? string.Empty;
          if (!string.IsNullOrEmpty(key) && !task.Inputs.ContainsKey(key))
            task.Inputs[key] = taskInputDefinition.DefaultValue?.Trim() ?? string.Empty;
        }
        context.Variables[WellKnownDistributedTaskVariables.TaskInstanceId] = (VariableValue) task.Id.ToString("D");
        context.Variables[WellKnownDistributedTaskVariables.TaskDisplayName] = (VariableValue) (task.DisplayName ?? task.Name);
        context.Variables[WellKnownDistributedTaskVariables.TaskInstanceName] = (VariableValue) task.Name;
        foreach (KeyValuePair<string, string> keyValuePair in task.Inputs.ToArray<KeyValuePair<string, string>>())
          task.Inputs[keyValuePair.Key] = context.ExpandVariables(keyValuePair.Value, false);
      }
      return new CreateTaskResult(task, definition);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<Demand> demands = this.m_demands;
      // ISSUE: explicit non-virtual call
      if ((demands != null ? (__nonvirtual (demands.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_demands = (List<Demand>) null;
      List<JobStep> steps = this.m_steps;
      // ISSUE: explicit non-virtual call
      if ((steps != null ? (__nonvirtual (steps.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_steps = (List<JobStep>) null;
      List<IVariable> variables = this.m_variables;
      // ISSUE: explicit non-virtual call
      if ((variables != null ? (__nonvirtual (variables.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_variables = (List<IVariable>) null;
    }
  }
}
