// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobScheduleTemplate
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationJobScheduleTemplate : ITeamFoundationJobScheduleTemplate
  {
    [XmlAttribute]
    public DateTime BaseTime { get; set; }

    [XmlIgnore]
    public TimeSpan StaggerInterval { get; set; }

    [XmlIgnore]
    public TimeSpan ScheduleInterval { get; set; }

    [XmlAttribute]
    public string TimeZoneId { get; set; } = TimeZoneInfo.Utc.Id;

    [XmlAttribute]
    public JobPriorityLevel PriorityLevel { get; set; } = TeamFoundationJobSchedule.DefaultPriorityLevel;

    public override string ToString() => string.Format("[BaseTime={0} StaggerInterval={1} ScheduleInterval={2}]", (object) this.BaseTime, (object) this.StaggerInterval, (object) this.ScheduleInterval);

    [XmlAttribute("StaggerInterval")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string __InternalStaggerInterval__
    {
      get => this.StaggerInterval.ToString();
      set => this.StaggerInterval = TimeSpan.Parse(value);
    }

    [XmlAttribute("ScheduleInterval")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string __InternalScheduleInterval__
    {
      get => this.ScheduleInterval.ToString();
      set => this.ScheduleInterval = TimeSpan.Parse(value);
    }
  }
}
