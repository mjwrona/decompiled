// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobCompletedEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class JobCompletedEvent : JobEvent
  {
    internal JobCompletedEvent()
      : base("JobCompleted")
    {
    }

    public JobCompletedEvent(Guid jobId, TaskResult result)
      : this(0L, jobId, result)
    {
    }

    public JobCompletedEvent(long requestId, Guid jobId, TaskResult result)
      : this(requestId, jobId, result, false)
    {
    }

    public JobCompletedEvent(
      long requestId,
      Guid jobId,
      TaskResult result,
      bool agentShuttingDown)
      : base("JobCompleted", jobId)
    {
      this.RequestId = requestId;
      this.Result = result;
      this.AgentShuttingDown = agentShuttingDown;
    }

    [DataMember(EmitDefaultValue = false)]
    public long RequestId { get; set; }

    [DataMember]
    public TaskResult Result { get; set; }

    [DataMember]
    public bool AgentShuttingDown { get; set; }
  }
}
