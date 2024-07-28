// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.TeamSettingUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  public static class TeamSettingUtils
  {
    public static Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior ToServerBugsBehavior(
      this Microsoft.TeamFoundation.Work.WebApi.BugsBehavior bugsBehavior)
    {
      switch (bugsBehavior)
      {
        case Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.Off:
          return Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior.Off;
        case Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.AsRequirements:
          return Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior.AsRequirements;
        case Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.AsTasks:
          return Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior.AsTasks;
        default:
          throw new ArgumentException(nameof (bugsBehavior));
      }
    }

    public static Microsoft.TeamFoundation.Work.WebApi.BugsBehavior ToWebApiBugsBehavior(
      this Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior bugsBehavior)
    {
      Microsoft.TeamFoundation.Work.WebApi.BugsBehavior webApiBugsBehavior = Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.Off;
      switch (bugsBehavior)
      {
        case Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior.Off:
          webApiBugsBehavior = Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.Off;
          break;
        case Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior.AsRequirements:
          webApiBugsBehavior = Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.AsRequirements;
          break;
        case Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior.AsTasks:
          webApiBugsBehavior = Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.AsTasks;
          break;
      }
      return webApiBugsBehavior;
    }

    public static DayOfWeek[] GetRemainingDays(this DayOfWeek[] days) => Enumerable.Range(0, 7).Where<int>((Func<int, bool>) (index => !((IEnumerable<DayOfWeek>) days).Contains<DayOfWeek>((DayOfWeek) index))).Select<int, DayOfWeek>((Func<int, DayOfWeek>) (index => (DayOfWeek) index)).ToArray<DayOfWeek>();

    public static int GetWorkingDays(IEnumerable<Microsoft.TeamFoundation.Work.WebApi.DateRange> daysOff, ITeamWeekends weekends)
    {
      int workingDays = 0;
      foreach (Microsoft.TeamFoundation.Work.WebApi.DateRange dateRange in daysOff)
      {
        for (DateTime t1 = dateRange.Start; DateTime.Compare(t1, dateRange.End) <= 0; t1 = t1.AddDays(1.0))
        {
          if (!((IEnumerable<DayOfWeek>) weekends.Days).Contains<DayOfWeek>(t1.DayOfWeek))
            ++workingDays;
        }
      }
      return workingDays;
    }
  }
}
