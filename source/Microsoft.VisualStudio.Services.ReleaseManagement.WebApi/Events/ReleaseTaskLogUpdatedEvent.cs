// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.ReleaseTaskLogUpdatedEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  [DataContract]
  public class ReleaseTaskLogUpdatedEvent : RealtimeReleaseEvent
  {
    public ReleaseTaskLogUpdatedEvent(
      Guid projectId,
      int releaseId,
      int environmentId,
      Guid timelineRecordId,
      IEnumerable<string> lines)
      : this(projectId, releaseId, environmentId, timelineRecordId, Guid.Empty, lines)
    {
    }

    public ReleaseTaskLogUpdatedEvent(
      Guid projectId,
      int releaseId,
      int environmentId,
      Guid jobTimelineRecordId,
      Guid stepTimelineRecordId,
      IEnumerable<string> lines)
      : base(projectId, releaseId, environmentId)
    {
      this.TimelineRecordId = jobTimelineRecordId;
      this.StepRecordId = stepTimelineRecordId;
      this.Lines = (IList<string>) new List<string>(lines);
    }

    [DataMember]
    public Guid TimelineRecordId { get; private set; }

    [DataMember(IsRequired = false)]
    public Guid StepRecordId { get; private set; }

    [DataMember]
    public IList<string> Lines { get; private set; }
  }
}
