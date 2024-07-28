// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TaskOrchestrationJob : TaskOrchestrationItem
  {
    [DataMember(Name = "Demands", EmitDefaultValue = false)]
    private List<Demand> m_demands;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private IDictionary<string, string> m_variables;
    [DataMember(Name = "Tasks", EmitDefaultValue = false)]
    private List<TaskInstance> m_tasks;

    internal TaskOrchestrationJob()
      : base(TaskOrchestrationItemType.Job)
    {
      this.ExecutionMode = "Agent";
    }

    public TaskOrchestrationJob(
      Guid instanceId,
      string name,
      string refName,
      string executionMode = "Agent")
      : base(TaskOrchestrationItemType.Job)
    {
      this.InstanceId = instanceId;
      this.Name = name;
      this.RefName = refName;
      this.ExecutionMode = executionMode;
    }

    private TaskOrchestrationJob(TaskOrchestrationJob jobToBeCloned)
      : base(jobToBeCloned.ItemType)
    {
      this.InstanceId = jobToBeCloned.InstanceId;
      this.Name = jobToBeCloned.Name;
      this.RefName = jobToBeCloned.RefName;
      this.ExecutionMode = jobToBeCloned.ExecutionMode;
      this.ExecutionTimeout = jobToBeCloned.ExecutionTimeout;
      if (jobToBeCloned.ExecuteAs != null)
        this.ExecuteAs = new IdentityRef()
        {
          DisplayName = jobToBeCloned.ExecuteAs.DisplayName,
          Id = jobToBeCloned.ExecuteAs.Id,
          ImageUrl = jobToBeCloned.ExecuteAs.ImageUrl,
          IsAadIdentity = jobToBeCloned.ExecuteAs.IsAadIdentity,
          IsContainer = jobToBeCloned.ExecuteAs.IsContainer,
          ProfileUrl = jobToBeCloned.ExecuteAs.ProfileUrl,
          UniqueName = jobToBeCloned.ExecuteAs.UniqueName,
          Url = jobToBeCloned.ExecuteAs.Url
        };
      if (jobToBeCloned.m_demands != null)
        this.m_demands = jobToBeCloned.Demands.Select<Demand, Demand>((Func<Demand, Demand>) (x => x.Clone())).ToList<Demand>();
      if (jobToBeCloned.m_variables != null)
        this.m_variables = (IDictionary<string, string>) new Dictionary<string, string>(jobToBeCloned.m_variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (jobToBeCloned.m_tasks == null)
        return;
      this.m_tasks = jobToBeCloned.m_tasks.Select<TaskInstance, TaskInstance>((Func<TaskInstance, TaskInstance>) (x => (TaskInstance) x.Clone())).ToList<TaskInstance>();
    }

    [DataMember]
    public Guid InstanceId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string RefName { get; set; }

    [DataMember]
    public string ExecutionMode { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ExecuteAs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TimeSpan? ExecutionTimeout { get; set; }

    public List<Demand> Demands
    {
      get
      {
        if (this.m_demands == null)
          this.m_demands = new List<Demand>();
        return this.m_demands;
      }
    }

    public IDictionary<string, string> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
    }

    public List<TaskInstance> Tasks
    {
      get
      {
        if (this.m_tasks == null)
          this.m_tasks = new List<TaskInstance>();
        return this.m_tasks;
      }
    }

    public TaskOrchestrationJob Clone() => new TaskOrchestrationJob(this);
  }
}
