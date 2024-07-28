// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy.JobRequestMessage
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy
{
  [DataContract]
  [JsonConverter(typeof (LegacyJsonConverter<JobRequestMessage>))]
  public sealed class JobRequestMessage
  {
    public static readonly string MessageType = "JobRequest";
    [DataMember(Order = 9, Name = "Tasks", EmitDefaultValue = false)]
    private List<TaskInstance> m_tasks;

    [JsonConstructor]
    public JobRequestMessage()
    {
    }

    public JobRequestMessage(
      TaskOrchestrationPlanReference plan,
      TimelineReference timeline,
      Guid jobId,
      string jobName,
      JobEnvironment environment,
      JobAuthorization authorization,
      IEnumerable<TaskInstance> tasks)
    {
      this.Plan = plan;
      this.JobId = jobId;
      this.JobName = jobName;
      this.Timeline = timeline;
      this.Environment = environment;
      this.Authorization = authorization;
      this.m_tasks = new List<TaskInstance>(tasks);
    }

    [DataMember(Order = 0)]
    public long RequestId { get; internal set; }

    [DataMember(Order = 1)]
    public Guid LockToken { get; internal set; }

    [DataMember(Order = 2)]
    public DateTime LockedUntil { get; internal set; }

    [DataMember(Order = 3)]
    public TaskOrchestrationPlanReference Plan { get; private set; }

    [DataMember(Order = 4)]
    public TimelineReference Timeline { get; private set; }

    [DataMember(Order = 5)]
    public Guid JobId { get; private set; }

    [DataMember(Order = 6)]
    public string JobName { get; private set; }

    [DataMember(Order = 7)]
    public JobEnvironment Environment { get; private set; }

    [DataMember(Order = 8)]
    public JobAuthorization Authorization { get; private set; }

    public ReadOnlyCollection<TaskInstance> Tasks
    {
      get
      {
        if (this.m_tasks == null)
          this.m_tasks = new List<TaskInstance>();
        return this.m_tasks.AsReadOnly();
      }
    }
  }
}
