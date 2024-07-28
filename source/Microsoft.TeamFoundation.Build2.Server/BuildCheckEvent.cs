// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildCheckEvent
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildCheckEvent
  {
    public BuildCheckEvent() => this.Attempts = (byte) 0;

    public Guid ProjectId { get; set; }

    public long CheckEventId { get; set; }

    public int BuildId { get; set; }

    public CheckEventType EventType { get; set; }

    public IBuildCheckEventPayload Payload { get; set; }

    public CheckEventStatus Status { get; set; }

    public DateTime CreatedTime { get; set; }

    public DateTime ScheduledTime { get; set; }

    public byte Attempts { get; set; }
  }
}
