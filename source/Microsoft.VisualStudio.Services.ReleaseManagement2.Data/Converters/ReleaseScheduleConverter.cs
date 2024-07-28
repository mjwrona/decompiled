// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseScheduleConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseScheduleConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule releaseSchedule)
    {
      if (releaseSchedule == null)
        throw new ArgumentNullException(nameof (releaseSchedule));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule()
      {
        JobId = releaseSchedule.JobId,
        TimeZoneId = releaseSchedule.TimeZoneId,
        StartHours = releaseSchedule.StartHours,
        StartMinutes = releaseSchedule.StartMinutes,
        DaysToRelease = releaseSchedule.DaysToRelease.ToWebApi(),
        ScheduleOnlyWithChanges = releaseSchedule.ScheduleOnlyWithChanges
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule releaseSchedule)
    {
      if (releaseSchedule == null)
        throw new ArgumentNullException(nameof (releaseSchedule));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule()
      {
        JobId = releaseSchedule.JobId,
        TimeZoneId = releaseSchedule.TimeZoneId,
        StartHours = releaseSchedule.StartHours,
        StartMinutes = releaseSchedule.StartMinutes,
        DaysToRelease = releaseSchedule.DaysToRelease.FromWebApi(),
        ScheduleOnlyWithChanges = releaseSchedule.ScheduleOnlyWithChanges
      };
    }
  }
}
