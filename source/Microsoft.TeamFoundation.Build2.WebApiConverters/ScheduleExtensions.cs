// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.ScheduleExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class ScheduleExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.Schedule ToWebApiSchedule(
      this Microsoft.TeamFoundation.Build2.Server.Schedule srvSchedule,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvSchedule == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Schedule) null;
      return new Microsoft.TeamFoundation.Build.WebApi.Schedule(securedObject)
      {
        TimeZoneId = srvSchedule.TimeZoneId,
        StartHours = srvSchedule.StartHours,
        StartMinutes = srvSchedule.StartMinutes,
        DaysToBuild = (Microsoft.TeamFoundation.Build.WebApi.ScheduleDays) srvSchedule.DaysToBuild,
        ScheduleJobId = srvSchedule.ScheduleJobId,
        BranchFilters = srvSchedule.BranchFilters,
        ScheduleOnlyWithChanges = srvSchedule.ScheduleOnlyWithChanges
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.Schedule ToServerSchedule(
      this Microsoft.TeamFoundation.Build.WebApi.Schedule webApiSchedule)
    {
      if (webApiSchedule == null)
        return (Microsoft.TeamFoundation.Build2.Server.Schedule) null;
      return new Microsoft.TeamFoundation.Build2.Server.Schedule()
      {
        TimeZoneId = webApiSchedule.TimeZoneId,
        StartHours = webApiSchedule.StartHours,
        StartMinutes = webApiSchedule.StartMinutes,
        DaysToBuild = (Microsoft.TeamFoundation.Build2.Server.ScheduleDays) webApiSchedule.DaysToBuild,
        ScheduleJobId = webApiSchedule.ScheduleJobId,
        BranchFilters = webApiSchedule.BranchFilters,
        ScheduleOnlyWithChanges = webApiSchedule.ScheduleOnlyWithChanges
      };
    }
  }
}
