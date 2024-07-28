// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent21
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent21 : JobQueueComponent20
  {
    private static readonly SqlMetaData[] typ_JobScheduleUpdateTable4 = new SqlMetaData[6]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ScheduledTime", SqlDbType.DateTime2),
      new SqlMetaData("ScheduleInterval", SqlDbType.Int),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 32L),
      new SqlMetaData("PriorityLevel", SqlDbType.Int)
    };

    internal override void ReleaseJobs(
      Guid agentId,
      int assumeHostActiveSeconds,
      int failureIgnoreDormancySeconds,
      int notificationRequiredSeconds,
      List<ReleaseJobInfo> jobsToRelease,
      List<TeamFoundationJobSchedule> jobsToReleaseSchedules,
      bool logSuccessfulJobs = true)
    {
      this.PrepareStoredProcedure("prc_ReleaseJobs");
      this.BindGuid("@agentId", agentId);
      this.BindInt("@assumeHostActiveSeconds", assumeHostActiveSeconds);
      this.BindInt("@failureIgnoreDormancySeconds", failureIgnoreDormancySeconds);
      this.BindInt("@notificationRequiredSeconds", notificationRequiredSeconds);
      this.BindReleaseJobsTable("@jobsToRelease", (IEnumerable<ReleaseJobInfo>) jobsToRelease);
      this.BindJobScheduleUpdateTable("@jobsToReleaseSchedules", (IEnumerable<TeamFoundationJobSchedule>) jobsToReleaseSchedules);
      this.BindBoolean("@logSuccessfulJobs", logSuccessfulJobs);
      int num = (int) this.ExecuteScalar();
      if (num == jobsToRelease.Count)
        return;
      this.Trace(1700, TraceLevel.Error, "Released {0} jobs instead of {1}", (object) num, (object) jobsToRelease.Count);
    }

    public override void UpdateJobQueue(
      Guid jobSource,
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates,
      IEnumerable<TeamFoundationJobSchedule> schedulesToUpdate)
    {
      this.PrepareStoredProcedure("prc_UpdateJobQueue");
      this.BindGuidTable("@queueRemovals", jobsToDelete);
      this.BindJobDefinitionUpdateTable("@jobUpdates", jobUpdates);
      this.BindJobScheduleUpdateTable("@scheduleUpdates", schedulesToUpdate);
      this.BindGuid("@jobSource", jobSource);
      this.ExecuteNonQuery();
    }

    protected override SqlParameter BindJobScheduleUpdateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobSchedule> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobSchedule>();
      System.Func<TeamFoundationJobSchedule, SqlDataRecord> selector = (System.Func<TeamFoundationJobSchedule, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent21.typ_JobScheduleUpdateTable4);
        if (row.JobSource == Guid.Empty)
          TeamFoundationTracingService.TraceRaw(2032130, TraceLevel.Error, "Job", nameof (JobQueueComponent21), "jobSchedule.JobSource is an empty guid.");
        sqlDataRecord.SetGuid(0, row.JobSource);
        sqlDataRecord.SetGuid(1, row.JobId);
        sqlDataRecord.SetDateTime(2, row.ScheduledTime.ToUniversalTime());
        sqlDataRecord.SetInt32(3, row.Interval);
        sqlDataRecord.SetString(4, row.TimeZoneId);
        if (row.PriorityLevel == JobPriorityLevel.None)
        {
          TeamFoundationTracingService.TraceRaw(2032131, TraceLevel.Error, "Job", nameof (JobQueueComponent21), "jobSchedule.PriorityLevel is None");
          row.PriorityLevel = JobPriorityLevel.BelowNormal;
        }
        sqlDataRecord.SetInt32(5, (int) row.PriorityLevel);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobScheduleUpdateTable4", rows.Select<TeamFoundationJobSchedule, SqlDataRecord>(selector));
    }
  }
}
