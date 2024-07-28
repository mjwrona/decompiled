// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TaskActivityData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  [DataContract]
  public class TaskActivityData
  {
    public TaskActivityData(
      Guid planId,
      Guid timelineId,
      Guid jobId,
      string jobName,
      Guid taskInstanceId,
      int taskIndex,
      string taskName)
    {
      this.PlanId = planId;
      this.TimelineId = timelineId;
      this.JobId = jobId;
      this.JobName = jobName;
      this.TaskInstanceId = taskInstanceId;
      this.TaskIndex = taskIndex;
      this.TaskName = taskName;
    }

    [DataMember]
    public Guid PlanId { get; }

    [DataMember]
    public Guid TimelineId { get; }

    [DataMember]
    public Guid JobId { get; }

    [DataMember]
    public string JobName { get; }

    [DataMember]
    public Guid TaskInstanceId { get; }

    [DataMember]
    public int TaskIndex { get; }

    [DataMember]
    public string TaskName { get; }
  }
}
