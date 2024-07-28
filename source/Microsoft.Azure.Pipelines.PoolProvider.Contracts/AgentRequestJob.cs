// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Contracts.AgentRequestJob
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2F8171A-7EDF-4EAC-B6BB-DAF285412F1E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Contracts.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.PoolProvider.Contracts
{
  [DataContract]
  public class AgentRequestJob
  {
    public AgentRequestJob(
      int poolId,
      Guid projectId,
      Guid planId,
      string planType,
      AgentRequestJobOwnerReference run,
      AgentRequestJobOwnerReference definition,
      Guid jobId,
      string jobName,
      string jobContainer,
      IList<AgentRequestJobStep> jobSteps,
      IDictionary<string, string> jobSidecarContainers,
      IList<AgentRequestJobVariable> jobVariables)
    {
      this.PoolId = poolId;
      this.ProjectId = projectId;
      this.PlanId = planId;
      this.PlanType = planType;
      this.Run = run;
      this.Definition = definition;
      this.Job = new AgentRequestJob.AgentRequestJobData(jobId, jobName, jobContainer, jobSteps, jobSidecarContainers, jobVariables);
    }

    [DataMember(EmitDefaultValue = false)]
    public int PoolId { get; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ProjectId { get; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PlanId { get; }

    [DataMember(EmitDefaultValue = false)]
    public string PlanType { get; }

    [DataMember(EmitDefaultValue = false)]
    public AgentRequestJobOwnerReference Run { get; }

    [DataMember(EmitDefaultValue = false)]
    public AgentRequestJobOwnerReference Definition { get; }

    [DataMember(EmitDefaultValue = false)]
    public AgentRequestJob.AgentRequestJobData Job { get; }

    [DataContract]
    public class AgentRequestJobData
    {
      [DataMember(Name = "Steps", EmitDefaultValue = false)]
      private List<AgentRequestJobStep> m_steps;
      [DataMember(Name = "SidecarContainers", EmitDefaultValue = false)]
      private IDictionary<string, string> m_sidecarContainers;
      [DataMember(Name = "Variables", EmitDefaultValue = false)]
      private List<AgentRequestJobVariable> m_variables;

      public AgentRequestJobData(
        Guid id,
        string name,
        string container,
        IList<AgentRequestJobStep> steps,
        IDictionary<string, string> sidecarContainers,
        IList<AgentRequestJobVariable> variables)
      {
        this.Id = id;
        this.Name = name;
        this.Container = container;
        this.m_variables = new List<AgentRequestJobVariable>((IEnumerable<AgentRequestJobVariable>) variables);
        this.m_steps = new List<AgentRequestJobStep>((IEnumerable<AgentRequestJobStep>) steps);
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

      public IList<AgentRequestJobStep> Steps
      {
        get
        {
          if (this.m_steps == null)
            this.m_steps = new List<AgentRequestJobStep>();
          return (IList<AgentRequestJobStep>) this.m_steps;
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

      public IList<AgentRequestJobVariable> Variables
      {
        get
        {
          if (this.m_variables == null)
            this.m_variables = new List<AgentRequestJobVariable>();
          return (IList<AgentRequestJobVariable>) this.m_variables;
        }
      }

      [OnSerializing]
      private void OnSerializing(StreamingContext context)
      {
        IDictionary<string, string> sidecarContainers = this.m_sidecarContainers;
        if ((sidecarContainers != null ? (sidecarContainers.Count == 0 ? 1 : 0) : 0) == 0)
          return;
        this.m_sidecarContainers = (IDictionary<string, string>) null;
      }
    }
  }
}
