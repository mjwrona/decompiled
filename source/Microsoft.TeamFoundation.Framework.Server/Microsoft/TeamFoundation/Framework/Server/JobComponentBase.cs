// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobComponentBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobComponentBase : TeamFoundationSqlResourceComponent
  {
    private static readonly SqlMetaData[] typ_QueryJobsTable = new SqlMetaData[2]
    {
      new SqlMetaData("JobIndex", SqlDbType.Int),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier)
    };
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static JobComponentBase()
    {
      JobComponentBase.s_sqlExceptionFactories.Add(800021, new SqlExceptionFactory(typeof (DuplicateJobIdException)));
      JobComponentBase.s_sqlExceptionFactories.Add(800022, new SqlExceptionFactory(typeof (DuplicateJobScheduleException)));
      JobComponentBase.s_sqlExceptionFactories.Add(800019, new SqlExceptionFactory(typeof (JobDidntPauseException)));
      JobComponentBase.s_sqlExceptionFactories.Add(800018, new SqlExceptionFactory(typeof (JobCannotBeStoppedException)));
      JobComponentBase.s_sqlExceptionFactories.Add(800024, new SqlExceptionFactory(typeof (JobCannotBePausedException)));
      JobComponentBase.s_sqlExceptionFactories.Add(800025, new SqlExceptionFactory(typeof (JobCannotBeResumedException)));
      JobComponentBase.s_sqlExceptionFactories.Add(800098, new SqlExceptionFactory(typeof (JobDefinitionUpdatesNotPermittedException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) JobComponentBase.s_sqlExceptionFactories;

    protected SqlParameter BindQueryJobsTable(string parameterName, IEnumerable<Guid> rows)
    {
      rows = rows ?? Enumerable.Empty<Guid>();
      return this.BindTable(parameterName, "typ_QueryJobsTable", this.BindQueryJobsRows(rows));
    }

    private IEnumerable<SqlDataRecord> BindQueryJobsRows(IEnumerable<Guid> rows)
    {
      int index = 0;
      foreach (Guid row in rows)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobComponentBase.typ_QueryJobsTable);
        sqlDataRecord.SetInt32(0, index++);
        sqlDataRecord.SetGuid(1, row);
        yield return sqlDataRecord;
      }
    }

    protected static List<DaylightTransitionInfo> GetDaylightTransitions(
      IEnumerable<TeamFoundationJobSchedule> schedules)
    {
      List<DaylightTransitionInfo> daylightTransitions = new List<DaylightTransitionInfo>();
      if (schedules != null)
      {
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        DateTime utcNow = DateTime.UtcNow;
        foreach (TeamFoundationJobSchedule schedule in schedules)
        {
          if (!stringSet.Contains(schedule.TimeZoneId))
          {
            stringSet.Add(schedule.TimeZoneId);
            TimeZoneInfo systemTimeZoneById = TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZoneId);
            if (systemTimeZoneById.SupportsDaylightSavingTime)
              daylightTransitions.AddRange((IEnumerable<DaylightTransitionInfo>) DaylightTransitionInfo.GetDaylightTransitions(systemTimeZoneById, utcNow.Year - 1, utcNow.Year + 1));
          }
        }
      }
      return daylightTransitions;
    }

    protected static TimeSpan GetScheduledTimeDelta(TeamFoundationJobSchedule jobSchedule)
    {
      TimeZoneInfo systemTimeZoneById = TimeZoneInfo.FindSystemTimeZoneById(jobSchedule.TimeZoneId);
      if (systemTimeZoneById.SupportsDaylightSavingTime)
      {
        DateTime utcDateTime = TeamFoundationJobSchedule.GetUtcDateTime(jobSchedule.ScheduledTime);
        foreach (DaylightTransitionInfo daylightTransition in DaylightTransitionInfo.GetDaylightTransitions(systemTimeZoneById, utcDateTime.Year - 1, utcDateTime.Year + 1))
        {
          if (daylightTransition.StartDate <= utcDateTime && daylightTransition.EndDate >= utcDateTime)
            return daylightTransition.Delta;
        }
      }
      return TimeSpan.Zero;
    }
  }
}
