// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskCompletedEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TaskCompletedEvent : TaskEvent
  {
    public TaskCompletedEvent()
      : base("TaskCompleted")
    {
    }

    public TaskCompletedEvent(Guid jobId, Guid taskId, TaskResult taskResult, bool deliveryFailed = false)
      : base("TaskCompleted", jobId, taskId)
    {
      this.Result = taskResult;
      this.DeliveryFailed = deliveryFailed;
    }

    [DataMember]
    public TaskResult Result { get; set; }

    [DataMember]
    public bool DeliveryFailed { get; set; }
  }
}
