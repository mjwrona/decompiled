// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolMaintenanceJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentPoolMaintenanceJob
  {
    [DataMember(EmitDefaultValue = false, Name = "TargetAgents")]
    private List<TaskAgentPoolMaintenanceJobTargetAgent> m_targetAgents;

    internal TaskAgentPoolMaintenanceJob()
    {
    }

    [DataMember]
    public int JobId { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentPoolReference Pool { get; set; }

    [DataMember]
    public Guid OrchestrationId { get; internal set; }

    [DataMember]
    public int DefinitionId { get; set; }

    [DataMember]
    public TaskAgentPoolMaintenanceJobStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentPoolMaintenanceJobResult? Result { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? QueueTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef RequestedBy { get; internal set; }

    [DataMember]
    public int ErrorCount { get; internal set; }

    [DataMember]
    public int WarningCount { get; internal set; }

    [DataMember]
    public string LogsDownloadUrl { get; internal set; }

    public List<TaskAgentPoolMaintenanceJobTargetAgent> TargetAgents
    {
      get
      {
        if (this.m_targetAgents == null)
          this.m_targetAgents = new List<TaskAgentPoolMaintenanceJobTargetAgent>();
        return this.m_targetAgents;
      }
      internal set => this.m_targetAgents = value;
    }
  }
}
