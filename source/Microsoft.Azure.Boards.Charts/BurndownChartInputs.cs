// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.BurndownChartInputs
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.Azure.Boards.Charts.Cache;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.Charts
{
  public class BurndownChartInputs : ChartInputs
  {
    public const int c_defaultMaxWorkitemLimit = 1000;
    public const int c_defaultMaxIterationDaysLimit = 90;
    public const string c_backlogsTeamSettingsRootPath = "/Configuration/Application/Backlogs/Team";
    public const string c_maxBurnDownWorkItemQueryLimit = "/maxBurnDownWorkItemQueryLimit";
    public const string c_maxIterationFutureDaysLimit = "/Configuration/Application/Backlogs/Team/SprintsDirectoryAllPivotPageSize";
    private List<string> m_workItemTypes;
    private List<string> m_inProgressStates;
    private List<DayOfWeek> m_weekendDays;
    private TimeZoneInfo m_timeZoneInfo;

    public IterationProperties Iteration { get; set; }

    public List<string> WorkItemTypes
    {
      get
      {
        if (this.m_workItemTypes == null)
          this.m_workItemTypes = new List<string>();
        return this.m_workItemTypes;
      }
      set => this.m_workItemTypes = value;
    }

    public string RemainingWorkField { get; set; }

    public List<string> InProgressStates
    {
      get
      {
        if (this.m_inProgressStates == null)
          this.m_inProgressStates = new List<string>();
        return this.m_inProgressStates;
      }
      set => this.m_inProgressStates = value;
    }

    public List<DayOfWeek> WeekendDays
    {
      get
      {
        if (this.m_weekendDays == null)
          this.m_weekendDays = new List<DayOfWeek>();
        return this.m_weekendDays;
      }
      set => this.m_weekendDays = value;
    }

    public List<DateRange> TeamCapacityOffDays { get; set; }

    public IEnumerable<TeamMemberCapacity> TeamMemberCapacityCollection { get; set; }

    public TimeZoneInfo TimeZone
    {
      get => this.m_timeZoneInfo == null ? TimeZoneInfo.Local : this.m_timeZoneInfo;
      set => this.m_timeZoneInfo = value;
    }

    public IdentityChartCache IdentityChartCache { get; set; }

    public bool EnforceLimit { get; set; }

    public int WorkItemCountLimit { get; set; }

    public bool IsNonWorkingDay(DateTime date)
    {
      if (this.WeekendDays.Contains(date.DayOfWeek))
        return true;
      return this.TeamCapacityOffDays != null && BurndownChartInputs.IsDayInRange((IList<DateRange>) this.TeamCapacityOffDays, date);
    }

    public List<DateTime> GetWorkingDays()
    {
      List<DateTime> workingDays = new List<DateTime>();
      for (DateTime date = this.Iteration.StartDate.Value; date <= this.Iteration.FinishDate.Value; date = date.AddDays(1.0))
      {
        if (!this.IsNonWorkingDay(date))
          workingDays.Add(date);
      }
      return workingDays;
    }

    public static bool IsDayInRange(IList<DateRange> dateRanges, DateTime date)
    {
      bool flag = false;
      foreach (DateRange dateRange in (IEnumerable<DateRange>) dateRanges)
      {
        if (date >= dateRange.Start && date <= dateRange.End)
        {
          flag = true;
          break;
        }
      }
      return flag;
    }
  }
}
