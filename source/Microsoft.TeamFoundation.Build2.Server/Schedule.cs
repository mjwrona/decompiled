// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Schedule
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class Schedule
  {
    private List<string> m_branchFilters;

    public string TimeZoneId { get; set; }

    public int StartHours { get; set; }

    public int StartMinutes { get; set; }

    public ScheduleDays DaysToBuild { get; set; }

    public Guid ScheduleJobId { get; set; }

    public List<string> BranchFilters
    {
      get
      {
        if (this.m_branchFilters == null)
          this.m_branchFilters = new List<string>();
        return this.m_branchFilters;
      }
      set => this.m_branchFilters = new List<string>((IEnumerable<string>) value);
    }

    public bool ScheduleOnlyWithChanges { get; set; }

    public Schedule Clone() => new Schedule()
    {
      TimeZoneId = this.TimeZoneId,
      StartHours = this.StartHours,
      StartMinutes = this.StartMinutes,
      DaysToBuild = this.DaysToBuild,
      ScheduleJobId = this.ScheduleJobId,
      BranchFilters = this.BranchFilters.ConvertAll<string>((Converter<string, string>) (filters => filters))
    };
  }
}
