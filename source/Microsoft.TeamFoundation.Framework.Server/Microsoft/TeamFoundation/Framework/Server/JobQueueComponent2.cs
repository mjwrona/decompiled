// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent2
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
  internal class JobQueueComponent2 : JobQueueComponent
  {
    private static readonly SqlMetaData[] typ_JobDefinitionUpdateTable2 = new SqlMetaData[3]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IgnoreDormancy", SqlDbType.Bit),
      new SqlMetaData("PriorityClass", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_JobQueueEntryTable2 = new SqlMetaData[5]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Priority", SqlDbType.Int),
      new SqlMetaData("QueueTime", SqlDbType.DateTime2),
      new SqlMetaData("QueuedReasonsValue", SqlDbType.Int),
      new SqlMetaData("QueueFlagsValue", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_JobQueueEntryWithJobSourceTable2 = new SqlMetaData[6]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Priority", SqlDbType.Int),
      new SqlMetaData("QueueTime", SqlDbType.DateTime2),
      new SqlMetaData("QueuedReasonsValue", SqlDbType.Int),
      new SqlMetaData("QueueFlagsValue", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_JobScheduleUpdateTable2 = new SqlMetaData[6]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ScheduledTime", SqlDbType.DateTime2),
      new SqlMetaData("ScheduleInterval", SqlDbType.Int),
      new SqlMetaData("ScheduleTimeDelta", SqlDbType.Float),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 260L),
      new SqlMetaData("PriorityLevel", SqlDbType.Int)
    };

    public override List<TeamFoundationJobHistoryEntry> QueryJobHistory(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryJobHistory");
      this.BindGuid("@jobSource", jobSource);
      this.BindGuidTable("@jobIds", jobIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobHistoryEntry>((ObjectBinder<TeamFoundationJobHistoryEntry>) new TeamFoundationJobHistoryEntryColumns2());
      return resultCollection.GetCurrent<TeamFoundationJobHistoryEntry>().Items;
    }

    public override List<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryLatestJobHistory");
      this.BindGuid("@jobSource", jobSource);
      this.BindQueryJobsTable("@jobIds", jobIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobHistoryEntry>((ObjectBinder<TeamFoundationJobHistoryEntry>) new TeamFoundationJobHistoryEntryColumns2());
      return resultCollection.GetCurrent<TeamFoundationJobHistoryEntry>().Items;
    }

    public override List<TeamFoundationJobQueueEntry> QueryJobQueueForOneHost(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryJobQueueForOneHost");
      this.BindGuid("@jobSource", jobSource);
      this.BindQueryJobsTable("@jobIds", jobIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns2());
      return resultCollection.GetCurrent<TeamFoundationJobQueueEntry>().Items;
    }

    internal override ResultCollection QueryJobQueue()
    {
      this.PrepareStoredProcedure("prc_QueryJobQueue");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns2());
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns2());
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns2());
      return resultCollection;
    }

    public override int QueueJobs(
      Guid jobSource,
      IEnumerable<Tuple<Guid, int>> jobsToRun,
      Guid requesterActivityId,
      Guid requesterVsid,
      JobPriorityLevel priorityLevel,
      int delaySeconds,
      bool queueAsDormant)
    {
      this.PrepareStoredProcedure("prc_QueueJobs");
      this.BindGuid("@jobSource", jobSource);
      this.BindGuidInt32Table("@jobList", jobsToRun);
      this.BindInt("@priorityLevel", (int) priorityLevel);
      this.BindInt("@delaySeconds", delaySeconds);
      return (int) this.ExecuteScalar();
    }

    protected override SqlParameter BindJobDefinitionUpdateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobDefinition>();
      System.Func<TeamFoundationJobDefinition, SqlDataRecord> selector = (System.Func<TeamFoundationJobDefinition, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent2.typ_JobDefinitionUpdateTable2);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetBoolean(1, row.IgnoreDormancy);
        if (row.PriorityClass == JobPriorityClass.None)
        {
          TeamFoundationTracingService.TraceRaw(2032120, TraceLevel.Error, "Job", nameof (JobQueueComponent2), "jobDefinition.PriorityClass is None");
          row.PriorityClass = JobPriorityClass.Normal;
        }
        sqlDataRecord.SetInt32(2, (int) row.PriorityClass);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobDefinitionUpdateTable2", rows.Select<TeamFoundationJobDefinition, SqlDataRecord>(selector));
    }

    protected override SqlParameter BindJobQueueEntryTable(
      string parameterName,
      IEnumerable<TeamFoundationJobQueueEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobQueueEntry>();
      System.Func<TeamFoundationJobQueueEntry, SqlDataRecord> selector = (System.Func<TeamFoundationJobQueueEntry, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent2.typ_JobQueueEntryTable2);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetInt32(1, row.Priority);
        sqlDataRecord.SetDateTime(2, row.QueueTime.ToUniversalTime());
        sqlDataRecord.SetInt32(3, row.QueuedReasonsValue);
        sqlDataRecord.SetInt32(4, row.QueueFlagsValue);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobQueueEntryTable2", rows.Select<TeamFoundationJobQueueEntry, SqlDataRecord>(selector));
    }

    protected override SqlParameter BindJobQueueEntryWithJobSourceTable(
      string parameterName,
      IEnumerable<TeamFoundationJobQueueEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobQueueEntry>();
      System.Func<TeamFoundationJobQueueEntry, SqlDataRecord> selector = (System.Func<TeamFoundationJobQueueEntry, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent2.typ_JobQueueEntryWithJobSourceTable2);
        sqlDataRecord.SetGuid(0, row.JobSource);
        sqlDataRecord.SetGuid(1, row.JobId);
        sqlDataRecord.SetInt32(2, row.Priority);
        sqlDataRecord.SetDateTime(3, row.QueueTime.ToUniversalTime());
        sqlDataRecord.SetInt32(4, row.QueuedReasonsValue);
        sqlDataRecord.SetInt32(5, row.QueueFlagsValue);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobQueueEntryWithJobSourceTable2", rows.Select<TeamFoundationJobQueueEntry, SqlDataRecord>(selector));
    }

    protected override SqlParameter BindJobScheduleUpdateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobSchedule> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobSchedule>();
      System.Func<TeamFoundationJobSchedule, SqlDataRecord> selector = (System.Func<TeamFoundationJobSchedule, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent2.typ_JobScheduleUpdateTable2);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetDateTime(1, row.ScheduledTime.ToUniversalTime());
        sqlDataRecord.SetInt32(2, row.Interval);
        sqlDataRecord.SetDouble(3, JobComponentBase.GetScheduledTimeDelta(row).TotalSeconds);
        sqlDataRecord.SetString(4, row.TimeZoneId);
        if (row.PriorityLevel == JobPriorityLevel.None)
        {
          TeamFoundationTracingService.TraceRaw(2032121, TraceLevel.Error, "Job", nameof (JobQueueComponent2), "jobSchedule.PriorityLevel is None");
          row.PriorityLevel = JobPriorityLevel.BelowNormal;
        }
        sqlDataRecord.SetInt32(5, (int) row.PriorityLevel);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobScheduleUpdateTable2", rows.Select<TeamFoundationJobSchedule, SqlDataRecord>(selector));
    }
  }
}
