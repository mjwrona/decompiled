// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent4
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
  internal class JobQueueComponent4 : JobQueueComponent3
  {
    private static readonly SqlMetaData[] typ_JobScheduleUpdateTable3 = new SqlMetaData[7]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ScheduledTime", SqlDbType.DateTime2),
      new SqlMetaData("ScheduleInterval", SqlDbType.Int),
      new SqlMetaData("ScheduleTimeDelta", SqlDbType.Float),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 260L),
      new SqlMetaData("PriorityLevel", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_ReleaseJobsTable = new SqlMetaData[6]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobResult", SqlDbType.TinyInt),
      new SqlMetaData("JobResultMessage", SqlDbType.NVarChar, -1L),
      new SqlMetaData("JobDefinitionExists", SqlDbType.Bit),
      new SqlMetaData("PriorityClass", SqlDbType.Int)
    };

    internal override ResultCollection AcquireJobs(
      Guid agentId,
      int maxJobsToAcquire,
      bool allowDeferJobs,
      int dormancyInterval)
    {
      this.PrepareStoredProcedure("prc_AcquireJobs");
      this.BindGuid("@agentId", agentId);
      this.BindInt("@maxJobsToAcquire", maxJobsToAcquire);
      this.BindBoolean("@allowDeferJobs", allowDeferJobs);
      this.BindInt("@dormancyInterval", dormancyInterval);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns2());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new NextScheduledJobColumns());
      return resultCollection;
    }

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
      this.BindReleaseJobsTable("@jobsToRelease", (IEnumerable<ReleaseJobInfo>) jobsToRelease);
      this.BindJobScheduleUpdateTableForReleaseJobs("@jobsToReleaseSchedules", (IEnumerable<TeamFoundationJobSchedule>) jobsToReleaseSchedules);
      this.BindDaylightTransitionInfoTable("@daylightTransitions", (IEnumerable<DaylightTransitionInfo>) JobComponentBase.GetDaylightTransitions((IEnumerable<TeamFoundationJobSchedule>) jobsToReleaseSchedules));
      this.ExecuteNonQuery();
    }

    protected SqlParameter BindJobScheduleUpdateTableForReleaseJobs(
      string parameterName,
      IEnumerable<TeamFoundationJobSchedule> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobSchedule>();
      System.Func<TeamFoundationJobSchedule, SqlDataRecord> selector = (System.Func<TeamFoundationJobSchedule, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent4.typ_JobScheduleUpdateTable3);
        if (row.JobSource == Guid.Empty)
          TeamFoundationTracingService.TraceRaw(2032123, TraceLevel.Error, "Job", nameof (JobQueueComponent4), "jobSchedule.JobSource is an empty guid.");
        sqlDataRecord.SetGuid(0, row.JobSource);
        sqlDataRecord.SetGuid(1, row.JobId);
        sqlDataRecord.SetDateTime(2, row.ScheduledTime.ToUniversalTime());
        sqlDataRecord.SetInt32(3, row.Interval);
        sqlDataRecord.SetDouble(4, JobComponentBase.GetScheduledTimeDelta(row).TotalSeconds);
        sqlDataRecord.SetString(5, row.TimeZoneId);
        if (row.PriorityLevel == JobPriorityLevel.None)
        {
          TeamFoundationTracingService.TraceRaw(2032124, TraceLevel.Error, "Job", nameof (JobQueueComponent4), "jobSchedule.PriorityLevel is None");
          row.PriorityLevel = JobPriorityLevel.BelowNormal;
        }
        sqlDataRecord.SetInt32(6, (int) row.PriorityLevel);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobScheduleUpdateTable3", rows.Select<TeamFoundationJobSchedule, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindReleaseJobsTable(
      string parameterName,
      IEnumerable<ReleaseJobInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseJobInfo>();
      System.Func<ReleaseJobInfo, SqlDataRecord> selector = (System.Func<ReleaseJobInfo, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent4.typ_ReleaseJobsTable);
        TeamFoundationJobResult foundationJobResult = row.JobResult;
        if (row.ScheduleSeconds > 0)
        {
          foundationJobResult = TeamFoundationJobResult.JobInitializationError;
          TeamFoundationTracingService.TraceRaw(2032125, TraceLevel.Error, "Job", nameof (JobQueueComponent4), "Job will not be rescheduled even though it had an initialization error. Please requeue this job manually.");
        }
        if (row.JobSource == Guid.Empty)
          TeamFoundationTracingService.TraceRaw(2032126, TraceLevel.Error, "Job", nameof (JobQueueComponent4), "jobToRelease.JobSource is an empty guid.");
        sqlDataRecord.SetGuid(0, row.JobSource);
        sqlDataRecord.SetGuid(1, row.JobId);
        sqlDataRecord.SetByte(2, (byte) foundationJobResult);
        if (row.ResultMessage != null)
          sqlDataRecord.SetString(3, row.ResultMessage);
        sqlDataRecord.SetBoolean(4, row.JobDefinitionExists);
        sqlDataRecord.SetInt32(5, (int) row.PriorityClass);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ReleaseJobsTable", rows.Select<ReleaseJobInfo, SqlDataRecord>(selector));
    }
  }
}
