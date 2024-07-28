// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentJobRequestMessage
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
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
  public sealed class AgentJobRequestMessage
  {
    [DataMember(Name = "Mask", EmitDefaultValue = false)]
    private List<MaskHint> m_maskHints;
    [DataMember(Name = "Steps", EmitDefaultValue = false)]
    private List<JobStep> m_steps;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private IDictionary<string, VariableValue> m_variables;
    [DataMember(Name = "JobSidecarContainers", EmitDefaultValue = false)]
    private IDictionary<string, string> m_jobSidecarContainers;

    [JsonConstructor]
    internal AgentJobRequestMessage()
    {
    }

    public AgentJobRequestMessage(
      TaskOrchestrationPlanReference plan,
      TimelineReference timeline,
      Guid jobId,
      string jobDisplayName,
      string jobName,
      string jobContainer,
      IDictionary<string, string> jobSidecarContainers,
      IDictionary<string, VariableValue> variables,
      IList<MaskHint> maskHints,
      JobResources jobResources,
      WorkspaceOptions workspaceOptions,
      IEnumerable<JobStep> steps)
    {
      this.MessageType = "PipelineAgentJobRequest";
      this.Plan = plan;
      this.JobId = jobId;
      this.JobDisplayName = jobDisplayName;
      this.JobName = jobName;
      this.JobContainer = jobContainer;
      this.Timeline = timeline;
      this.Resources = jobResources;
      this.Workspace = workspaceOptions;
      this.m_variables = (IDictionary<string, VariableValue>) new Dictionary<string, VariableValue>(variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_maskHints = new List<MaskHint>((IEnumerable<MaskHint>) maskHints);
      this.m_steps = new List<JobStep>(steps);
      if (jobSidecarContainers == null || jobSidecarContainers.Count <= 0)
        return;
      this.m_jobSidecarContainers = (IDictionary<string, string>) new Dictionary<string, string>(jobSidecarContainers, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember]
    public string MessageType { get; private set; }

    [DataMember]
    public TaskOrchestrationPlanReference Plan { get; private set; }

    [DataMember]
    public TimelineReference Timeline { get; private set; }

    [DataMember]
    public Guid JobId { get; private set; }

    [DataMember]
    public string JobDisplayName { get; private set; }

    [DataMember]
    public string JobName { get; private set; }

    [DataMember]
    public string JobContainer { get; private set; }

    [DataMember]
    public long RequestId { get; internal set; }

    [DataMember]
    public DateTime LockedUntil { get; internal set; }

    [DataMember]
    public JobResources Resources { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkspaceOptions Workspace { get; private set; }

    public List<MaskHint> MaskHints
    {
      get
      {
        if (this.m_maskHints == null)
          this.m_maskHints = new List<MaskHint>();
        return this.m_maskHints;
      }
    }

    public IDictionary<string, VariableValue> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = (IDictionary<string, VariableValue>) new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
    }

    public IList<JobStep> Steps
    {
      get
      {
        if (this.m_steps == null)
          this.m_steps = new List<JobStep>();
        return (IList<JobStep>) this.m_steps;
      }
    }

    public IDictionary<string, string> JobSidecarContainers
    {
      get
      {
        if (this.m_jobSidecarContainers == null)
          this.m_jobSidecarContainers = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_jobSidecarContainers;
      }
    }

    public TaskAgentMessage GetAgentMessage()
    {
      string str = JsonUtility.ToString((object) this);
      return new TaskAgentMessage()
      {
        Body = str,
        MessageType = "PipelineAgentJobRequest"
      };
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<MaskHint> maskHints = this.m_maskHints;
      // ISSUE: explicit non-virtual call
      this.m_maskHints = (maskHints != null ? (__nonvirtual (maskHints.Count) == 0 ? 1 : 0) : 0) == 0 ? new List<MaskHint>(this.m_maskHints.Distinct<MaskHint>()) : (List<MaskHint>) null;
      IDictionary<string, VariableValue> variables = this.m_variables;
      if ((variables != null ? (variables.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_variables = (IDictionary<string, VariableValue>) null;
      IDictionary<string, string> sidecarContainers = this.m_jobSidecarContainers;
      if ((sidecarContainers != null ? (sidecarContainers.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_jobSidecarContainers = (IDictionary<string, string>) null;
    }
  }
}
