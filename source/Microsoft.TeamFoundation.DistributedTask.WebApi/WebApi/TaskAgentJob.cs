// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentJob
  {
    [DataMember(Name = "Steps", EmitDefaultValue = false)]
    private List<TaskAgentJobStep> m_steps;
    [DataMember(Name = "SidecarContainers", EmitDefaultValue = false)]
    private IDictionary<string, string> m_sidecarContainers;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private List<TaskAgentJobVariable> m_variables;

    public TaskAgentJob(
      Guid id,
      string name,
      string container,
      IList<TaskAgentJobStep> steps,
      IDictionary<string, string> sidecarContainers,
      IList<TaskAgentJobVariable> variables)
    {
      this.Id = id;
      this.Name = name;
      this.Container = container;
      this.m_variables = new List<TaskAgentJobVariable>((IEnumerable<TaskAgentJobVariable>) variables);
      this.m_steps = new List<TaskAgentJobStep>((IEnumerable<TaskAgentJobStep>) steps);
      if (sidecarContainers == null || sidecarContainers.Count <= 0)
        return;
      this.m_sidecarContainers = (IDictionary<string, string>) new Dictionary<string, string>(sidecarContainers, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember]
    public Guid Id { get; }

    [DataMember]
    public string Name { get; }

    [DataMember(EmitDefaultValue = false)]
    public string Container { get; }

    public IList<TaskAgentJobStep> Steps
    {
      get
      {
        if (this.m_steps == null)
          this.m_steps = new List<TaskAgentJobStep>();
        return (IList<TaskAgentJobStep>) this.m_steps;
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

    public IList<TaskAgentJobVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new List<TaskAgentJobVariable>();
        return (IList<TaskAgentJobVariable>) this.m_variables;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IDictionary<string, string> sidecarContainers = this.m_sidecarContainers;
      if ((sidecarContainers != null ? (sidecarContainers.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_sidecarContainers = (IDictionary<string, string>) null;
    }
  }
}
