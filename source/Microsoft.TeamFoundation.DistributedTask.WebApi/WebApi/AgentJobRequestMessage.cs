// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.AgentJobRequestMessage
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class AgentJobRequestMessage : JobRequestMessage
  {
    [DataMember(Name = "Tasks", EmitDefaultValue = false)]
    private List<TaskInstance> m_tasks;

    [JsonConstructor]
    internal AgentJobRequestMessage()
      : base("JobRequest")
    {
    }

    public AgentJobRequestMessage(
      TaskOrchestrationPlanReference plan,
      TimelineReference timeline,
      Guid jobId,
      string jobName,
      string jobRefName,
      JobEnvironment environment,
      IEnumerable<TaskInstance> tasks)
      : base("JobRequest", plan, timeline, jobId, jobName, jobRefName, environment)
    {
      this.m_tasks = new List<TaskInstance>(tasks);
    }

    [DataMember]
    public long RequestId { get; internal set; }

    [DataMember]
    public Guid LockToken { get; internal set; }

    [DataMember]
    public DateTime LockedUntil { get; internal set; }

    public ReadOnlyCollection<TaskInstance> Tasks
    {
      get
      {
        if (this.m_tasks == null)
          this.m_tasks = new List<TaskInstance>();
        return this.m_tasks.AsReadOnly();
      }
    }

    public TaskAgentMessage GetAgentMessage()
    {
      string str = JsonUtility.ToString((object) this);
      return new TaskAgentMessage()
      {
        Body = str,
        MessageType = "JobRequest"
      };
    }
  }
}
