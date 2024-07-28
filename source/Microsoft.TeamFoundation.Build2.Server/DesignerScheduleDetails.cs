// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DesignerScheduleDetails
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class DesignerScheduleDetails
  {
    public string TimeZoneId { get; set; }

    public int StartHours { get; set; }

    public int StartMinutes { get; set; }

    public ScheduleDays DaysToBuild { get; set; }

    public DesignerScheduleDetails(
      string timeZoneId,
      int startHours,
      int startMinutes,
      ScheduleDays daysToBuild)
    {
      this.TimeZoneId = timeZoneId;
      this.StartHours = startHours;
      this.StartMinutes = startMinutes;
      this.DaysToBuild = daysToBuild;
    }
  }
}
