// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Events.ConsoleLogEvent
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi.Events
{
  [DataContract]
  public sealed class ConsoleLogEvent : RealtimeBuildEvent
  {
    [DataMember(Name = "Lines")]
    private List<string> m_lines;

    public ConsoleLogEvent(
      int buildId,
      Guid timelineId,
      Guid jobTimelineRecordId,
      IEnumerable<string> lines)
      : this(buildId, timelineId, jobTimelineRecordId, Guid.Empty, lines)
    {
    }

    public ConsoleLogEvent(
      int buildId,
      Guid timelineId,
      Guid jobTimelineRecordId,
      Guid stepTimelineRecordId,
      IEnumerable<string> lines)
      : base(buildId)
    {
      this.TimelineId = timelineId;
      this.TimelineRecordId = jobTimelineRecordId;
      this.StepRecordId = stepTimelineRecordId;
      this.m_lines = new List<string>(lines);
    }

    [DataMember(IsRequired = true)]
    public Guid TimelineId { get; private set; }

    [DataMember(IsRequired = true)]
    public Guid TimelineRecordId { get; private set; }

    [DataMember(IsRequired = false)]
    public Guid StepRecordId { get; private set; }

    public List<string> Lines
    {
      get
      {
        if (this.m_lines == null)
          this.m_lines = new List<string>();
        return this.m_lines;
      }
    }
  }
}
