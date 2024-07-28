// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DefaultDateRanges
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public static class DefaultDateRanges
  {
    public static FixedIntervalDateRange Snapshot()
    {
      FixedIntervalDateRange intervalDateRange = new FixedIntervalDateRange();
      intervalDateRange.LabelText = ReportingResources.HistoryRangeLabel_Snapshot();
      intervalDateRange.Name = "";
      intervalDateRange.Interval = TimeSpan.FromDays(-1.0);
      intervalDateRange.SampleCount = 1;
      return intervalDateRange;
    }

    public static FixedIntervalDateRange Last7Days()
    {
      FixedIntervalDateRange intervalDateRange = new FixedIntervalDateRange();
      intervalDateRange.LabelText = ReportingResources.HistoryRangeLabel_Last7Days();
      intervalDateRange.Name = "last7Days";
      intervalDateRange.Interval = TimeSpan.FromDays(-1.0);
      intervalDateRange.SampleCount = 7;
      return intervalDateRange;
    }

    public static FixedIntervalDateRange Last2Weeks()
    {
      FixedIntervalDateRange intervalDateRange = new FixedIntervalDateRange();
      intervalDateRange.LabelText = ReportingResources.HistoryRangeLabel_Last2Weeks();
      intervalDateRange.Name = "last2Weeks";
      intervalDateRange.Interval = TimeSpan.FromDays(-1.0);
      intervalDateRange.SampleCount = 14;
      return intervalDateRange;
    }

    public static FixedIntervalDateRange Last4Weeks()
    {
      FixedIntervalDateRange intervalDateRange = new FixedIntervalDateRange();
      intervalDateRange.LabelText = ReportingResources.HistoryRangeLabel_Last4Weeks();
      intervalDateRange.Name = "last4Weeks";
      intervalDateRange.Interval = TimeSpan.FromDays(-1.0);
      intervalDateRange.SampleCount = 28;
      return intervalDateRange;
    }

    public static FixedIntervalDateRange Last12Weeks()
    {
      WeeklyDateRange weeklyDateRange = new WeeklyDateRange();
      weeklyDateRange.LabelText = ReportingResources.HistoryRangeLabel_Last12Weeks();
      weeklyDateRange.Name = "last12Weeks";
      weeklyDateRange.SampleCount = 12;
      return (FixedIntervalDateRange) weeklyDateRange;
    }

    public static FixedIntervalDateRange LastYear()
    {
      MonthlyDateRange monthlyDateRange = new MonthlyDateRange();
      monthlyDateRange.LabelText = ReportingResources.HistoryRangeLabel_LastYear();
      monthlyDateRange.Name = "lastYear";
      monthlyDateRange.SampleCount = 12;
      return (FixedIntervalDateRange) monthlyDateRange;
    }

    public static IEnumerable<FixedIntervalDateRange> GetDefaultOptions() => (IEnumerable<FixedIntervalDateRange>) new List<FixedIntervalDateRange>()
    {
      DefaultDateRanges.Last7Days(),
      DefaultDateRanges.Last2Weeks(),
      DefaultDateRanges.Last4Weeks(),
      DefaultDateRanges.Last12Weeks(),
      DefaultDateRanges.LastYear()
    };
  }
}
