// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent
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
  internal class JobQueueComponent : JobComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[24]
    {
      (IComponentCreator) new ComponentCreator<JobQueueComponent>(1),
      (IComponentCreator) new ComponentCreator<JobQueueComponent2>(2),
      (IComponentCreator) new ComponentCreator<JobQueueComponent3>(3),
      (IComponentCreator) new ComponentCreator<JobQueueComponent4>(4, true),
      (IComponentCreator) new ComponentCreator<JobQueueComponent5>(5),
      (IComponentCreator) new ComponentCreator<JobQueueComponent6>(6),
      (IComponentCreator) new ComponentCreator<JobQueueComponent7>(7),
      (IComponentCreator) new ComponentCreator<JobQueueComponent8>(8),
      (IComponentCreator) new ComponentCreator<JobQueueComponent9>(9),
      (IComponentCreator) new ComponentCreator<JobQueueComponent10>(10),
      (IComponentCreator) new ComponentCreator<JobQueueComponent11>(11),
      (IComponentCreator) new ComponentCreator<JobQueueComponent12>(12),
      (IComponentCreator) new ComponentCreator<JobQueueComponent13>(13),
      (IComponentCreator) new ComponentCreator<JobQueueComponent13>(14),
      (IComponentCreator) new ComponentCreator<JobQueueComponent15>(15),
      (IComponentCreator) new ComponentCreator<JobQueueComponent16>(16),
      (IComponentCreator) new ComponentCreator<JobQueueComponent17>(17),
      (IComponentCreator) new ComponentCreator<JobQueueComponent18>(18),
      (IComponentCreator) new ComponentCreator<JobQueueComponent19>(19),
      (IComponentCreator) new ComponentCreator<JobQueueComponent20>(20),
      (IComponentCreator) new ComponentCreator<JobQueueComponent21>(21),
      (IComponentCreator) new ComponentCreator<JobQueueComponent22>(22),
      (IComponentCreator) new ComponentCreator<JobQueueComponent23>(23),
      (IComponentCreator) new ComponentCreator<JobQueueComponent24>(24)
    }, "JobQueue");
    private static readonly SqlMetaData[] typ_JobDefinitionUpdateTable = new SqlMetaData[2]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IgnoreDormancy", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_JobQueueEntryTable = new SqlMetaData[5]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("HighPriority", SqlDbType.Bit),
      new SqlMetaData("QueueTime", SqlDbType.DateTime2),
      new SqlMetaData("QueuedReasonsValue", SqlDbType.Int),
      new SqlMetaData("QueueFlagsValue", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_JobQueueEntryWithJobSourceTable = new SqlMetaData[6]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("HighPriority", SqlDbType.Bit),
      new SqlMetaData("QueueTime", SqlDbType.DateTime2),
      new SqlMetaData("QueuedReasonsValue", SqlDbType.Int),
      new SqlMetaData("QueueFlagsValue", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_JobScheduleUpdateTable = new SqlMetaData[5]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ScheduledTime", SqlDbType.DateTime2),
      new SqlMetaData("ScheduleInterval", SqlDbType.Int),
      new SqlMetaData("ScheduleTimeDelta", SqlDbType.Float),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 260L)
    };
    private static readonly SqlMetaData[] typ_DaylightTransitionInfoTable = new SqlMetaData[4]
    {
      new SqlMetaData("TimezoneId", SqlDbType.NVarChar, 260L),
      new SqlMetaData("StartTime", SqlDbType.DateTime2),
      new SqlMetaData("EndTime", SqlDbType.DateTime2),
      new SqlMetaData("TimeDelta", SqlDbType.Float)
    };

    public void ChangeJobState(
      Guid agentId,
      Guid jobSource,
      Guid jobId,
      TeamFoundationJobState newState)
    {
      this.PrepareStoredProcedure("prc_ChangeJobState");
      this.BindGuid("@agentId", agentId);
      this.BindGuid("@jobSource", jobSource);
      this.BindGuid("@jobId", jobId);
      this.BindByte("@newState", (byte) newState);
      this.ExecuteNonQuery();
    }

    public bool StopJob(Guid jobSource, Guid jobId, int commandTimeout = 0)
    {
      if (commandTimeout == 0)
        this.PrepareStoredProcedure("prc_StopJob");
      else
        this.PrepareStoredProcedure("prc_StopJob", commandTimeout);
      this.BindGuid("@jobSource", jobSource);
      this.BindGuid("@jobId", jobId);
      return 1 == (int) this.ExecuteScalar();
    }

    public virtual int QueueJobs(
      Guid jobSource,
      IEnumerable<Tuple<Guid, int>> jobsToRun,
      Guid requesterActivityId,
      Guid requesterVsid,
      JobPriorityLevel priorityLevel,
      int maxDelaySeconds,
      bool queueAsDormant)
    {
      this.PrepareStoredProcedure("prc_QueueJobs");
      this.BindGuid("@jobSource", jobSource);
      this.BindGuidTable("@jobIdList", jobsToRun.Select<Tuple<Guid, int>, Guid>((System.Func<Tuple<Guid, int>, Guid>) (jobRef => jobRef.Item1)));
      this.BindBoolean("@highPriority", priorityLevel > JobPriorityLevel.Normal);
      this.BindInt("@maxDelaySeconds", maxDelaySeconds);
      this.Trace(1701, TraceLevel.Info, "JobQueue", (object) "Component", (object) string.Format("Queued {0} job(s) for host {1}. Queued as dormant: {2}", (object) jobsToRun.Count<Tuple<Guid, int>>(), (object) jobSource, (object) queueAsDormant));
      return (int) this.ExecuteScalar();
    }

    public virtual void UpdateJobQueue(
      Guid jobSource,
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates,
      IEnumerable<TeamFoundationJobSchedule> schedulesToUpdate)
    {
      this.PrepareStoredProcedure("prc_UpdateJobQueue");
      this.BindGuidTable("@queueRemovals", jobsToDelete);
      this.BindJobDefinitionUpdateTable("@jobUpdates", jobUpdates);
      this.BindJobScheduleUpdateTable("@scheduleUpdates", schedulesToUpdate);
      this.BindDaylightTransitionInfoTable("@daylightTransitions", (IEnumerable<DaylightTransitionInfo>) JobComponentBase.GetDaylightTransitions(schedulesToUpdate));
      this.BindGuid("@jobSource", jobSource);
      this.ExecuteNonQuery();
    }

    public virtual void ClearJobQueue(Guid jobSource)
    {
    }

    public virtual List<TeamFoundationJobHistoryEntry> QueryJobHistory(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryJobHistory", 3600);
      this.BindGuid("@jobSource", jobSource);
      this.BindGuidTable("@jobIds", jobIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobHistoryEntry>((ObjectBinder<TeamFoundationJobHistoryEntry>) new TeamFoundationJobHistoryEntryColumns());
      return resultCollection.GetCurrent<TeamFoundationJobHistoryEntry>().Items;
    }

    public virtual List<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryLatestJobHistory", 3600);
      this.BindGuid("@jobSource", jobSource);
      this.BindQueryJobsTable("@jobIds", jobIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobHistoryEntry>((ObjectBinder<TeamFoundationJobHistoryEntry>) new TeamFoundationJobHistoryEntryColumns());
      return resultCollection.GetCurrent<TeamFoundationJobHistoryEntry>().Items;
    }

    public virtual void CleanupJobHistory(int jobAge)
    {
      this.PrepareStoredProcedure("prc_CleanupJobHistory", 3600);
      this.BindNullableGuid("@jobSource", Guid.Empty);
      this.BindGuidTable("@jobDeletes", (IEnumerable<Guid>) null);
      this.BindNullableInt("@jobAge", jobAge, 0);
      this.ExecuteNonQuery();
    }

    public virtual void CleanupJobHistory(Guid jobSource)
    {
      this.PrepareStoredProcedure("prc_CleanupJobHistory", 3600);
      this.BindNullableGuid("@jobSource", jobSource);
      this.BindGuidTable("@jobDeletes", (IEnumerable<Guid>) null);
      this.BindNullableInt("@jobAge", 0, 0);
      this.ExecuteNonQuery();
    }

    public virtual List<TeamFoundationJobQueueEntry> QueryJobQueueForOneHost(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryJobQueueForOneHost");
      this.BindGuid("@jobSource", jobSource);
      this.BindQueryJobsTable("@jobIds", jobIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns());
      return resultCollection.GetCurrent<TeamFoundationJobQueueEntry>().Items;
    }

    internal virtual ResultCollection QueryJobQueue()
    {
      this.PrepareStoredProcedure("prc_QueryJobQueue");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns());
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns());
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns());
      return resultCollection;
    }

    internal virtual List<TeamFoundationJobQueueEntry> RescheduleJobs(
      IEnumerable<Guid> onlineProcesses)
    {
      this.PrepareStoredProcedure("prc_RescheduleJobs");
      this.ExecuteNonQuery();
      return new List<TeamFoundationJobQueueEntry>();
    }

    internal virtual ResultCollection AcquireJobs(
      Guid agentId,
      int maxJobsToAcquire,
      bool allowDeferJobs,
      int dormancyInterval)
    {
      throw new InvalidServiceVersionException("JobQueue", this.Version, 4);
    }

    internal virtual void ReleaseJobs(
      Guid agentId,
      int assumeHostActiveSeconds,
      int failureIgnoreDormancySeconds,
      int notificationRequiredSeconds,
      List<ReleaseJobInfo> jobsToRelease,
      List<TeamFoundationJobSchedule> jobsToReleaseSchedules,
      bool logSuccessfulJobs = true)
    {
      throw new InvalidServiceVersionException("JobQueue", this.Version, 4);
    }

    internal virtual void ReenableJobs(IEnumerable<Guid> jobSources) => this.Trace(2032119, TraceLevel.Info, "JobQueueComponent.ReenableJobs called, but prc_ReenableJobs won't exist at ServiceVersion {0}. Ignoring.", (object) this.Version);

    internal virtual List<PendingJobInfo> QueryPendingJobs() => new List<PendingJobInfo>();

    internal virtual List<TeamFoundationJobQueueEntry> QueryRunningJobs(bool includePendingJobs) => new List<TeamFoundationJobQueueEntry>();

    internal virtual int UpdateJobQueuePriorityDirect(
      IEnumerable<TeamFoundationJobQueueEntry> jobQueueUpdates)
    {
      return -1;
    }

    protected virtual SqlParameter BindJobDefinitionUpdateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobDefinition>();
      System.Func<TeamFoundationJobDefinition, SqlDataRecord> selector = (System.Func<TeamFoundationJobDefinition, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent.typ_JobDefinitionUpdateTable);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetBoolean(1, row.IgnoreDormancy);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobDefinitionUpdateTable", rows.Select<TeamFoundationJobDefinition, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindJobQueueEntryTable(
      string parameterName,
      IEnumerable<TeamFoundationJobQueueEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobQueueEntry>();
      System.Func<TeamFoundationJobQueueEntry, SqlDataRecord> selector = (System.Func<TeamFoundationJobQueueEntry, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent.typ_JobQueueEntryTable);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetBoolean(1, row.Priority > 0);
        sqlDataRecord.SetDateTime(2, row.QueueTime.ToUniversalTime());
        sqlDataRecord.SetInt32(3, row.QueuedReasonsValue);
        sqlDataRecord.SetInt32(4, row.QueueFlagsValue);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobQueueEntryTable", rows.Select<TeamFoundationJobQueueEntry, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindJobQueueEntryWithJobSourceTable(
      string parameterName,
      IEnumerable<TeamFoundationJobQueueEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobQueueEntry>();
      System.Func<TeamFoundationJobQueueEntry, SqlDataRecord> selector = (System.Func<TeamFoundationJobQueueEntry, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent.typ_JobQueueEntryWithJobSourceTable);
        sqlDataRecord.SetGuid(0, row.JobSource);
        sqlDataRecord.SetGuid(1, row.JobId);
        sqlDataRecord.SetBoolean(2, row.Priority > 0);
        sqlDataRecord.SetDateTime(3, row.QueueTime.ToUniversalTime());
        sqlDataRecord.SetInt32(4, row.QueuedReasonsValue);
        sqlDataRecord.SetInt32(5, row.QueueFlagsValue);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobQueueEntryWithJobSourceTable", rows.Select<TeamFoundationJobQueueEntry, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindJobScheduleUpdateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobSchedule> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobSchedule>();
      System.Func<TeamFoundationJobSchedule, SqlDataRecord> selector = (System.Func<TeamFoundationJobSchedule, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent.typ_JobScheduleUpdateTable);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetDateTime(1, row.ScheduledTime.ToUniversalTime());
        sqlDataRecord.SetInt32(2, row.Interval);
        sqlDataRecord.SetDouble(3, JobComponentBase.GetScheduledTimeDelta(row).TotalSeconds);
        sqlDataRecord.SetString(4, row.TimeZoneId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobScheduleUpdateTable", rows.Select<TeamFoundationJobSchedule, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindDaylightTransitionInfoTable(
      string parameterName,
      IEnumerable<DaylightTransitionInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<DaylightTransitionInfo>();
      System.Func<DaylightTransitionInfo, SqlDataRecord> selector = (System.Func<DaylightTransitionInfo, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent.typ_DaylightTransitionInfoTable);
        sqlDataRecord.SetString(0, row.TimeZoneId);
        sqlDataRecord.SetDateTime(1, row.StartDate.ToUniversalTime());
        sqlDataRecord.SetDateTime(2, row.EndDate.ToUniversalTime());
        sqlDataRecord.SetDouble(3, row.Delta.TotalSeconds);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_DaylightTransitionInfoTable", rows.Select<DaylightTransitionInfo, SqlDataRecord>(selector));
    }
  }
}
