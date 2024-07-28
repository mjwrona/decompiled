// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Events.TimelineRecordUpdate
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.SignalR;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server.Events
{
  [DataContract]
  public class TimelineRecordUpdate : ISignalRObject
  {
    public TimelineRecordUpdate(TimelineRecord record)
    {
      this.Id = record.Id;
      this.State = record.State.ToBuildTimelineRecordState().ToString();
      this.Result = record.Result.ToBuildTaskResult().ToString();
      this.StartTime = record.StartTime.ToString();
      this.FinishTime = record.FinishTime.ToString();
    }

    [DataMember]
    public Guid Id { get; }

    [DataMember]
    public string State { get; }

    [DataMember]
    public string Result { get; }

    [DataMember]
    public string StartTime { get; }

    [DataMember]
    public string FinishTime { get; }
  }
}
