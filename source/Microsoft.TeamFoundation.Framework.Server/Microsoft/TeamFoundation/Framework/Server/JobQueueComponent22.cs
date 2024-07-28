// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent22
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent22 : JobQueueComponent21
  {
    private static readonly SqlMetaData[] typ_JobQueueDeleteTable = new SqlMetaData[2]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_JobDefinitionUpdateTable4 = new SqlMetaData[6]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("PriorityClass", SqlDbType.Int),
      new SqlMetaData("IsTemplateJob", SqlDbType.Bit),
      new SqlMetaData("QueueAsDormant", SqlDbType.Bit),
      new SqlMetaData("OverrideQueueTime", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_ReleaseJobsTable3 = new SqlMetaData[9]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobResult", SqlDbType.TinyInt),
      new SqlMetaData("JobResultMessage", SqlDbType.NVarChar, -1L),
      new SqlMetaData("JobDefinitionExists", SqlDbType.Bit),
      new SqlMetaData("PriorityClass", SqlDbType.Int),
      new SqlMetaData("ScheduleSeconds", SqlDbType.Int),
      new SqlMetaData("IgnoreDormancySeconds", SqlDbType.Int),
      new SqlMetaData("IsTemplateJob", SqlDbType.Bit)
    };

    public override void UpdateJobQueue(
      Guid jobSource,
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates,
      IEnumerable<TeamFoundationJobSchedule> schedulesToUpdate)
    {
      this.PrepareStoredProcedure("prc_UpdateJobQueue");
      this.BindJobQueueRemovalTable("@queueRemovals", jobSource, jobsToDelete);
      this.BindJobDefinitionUpdateTable("@jobUpdates", jobSource, jobUpdates);
      this.BindJobScheduleUpdateTable("@scheduleUpdates", schedulesToUpdate);
      this.ExecuteNonQuery();
    }

    public override List<TeamFoundationJobHistoryEntry> QueryJobHistory(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryJobHistory");
      this.BindGuid("@jobSource", jobSource);
      this.BindGuidTable("@jobIds", jobIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationJobHistoryEntry>((ObjectBinder<TeamFoundationJobHistoryEntry>) new TeamFoundationJobHistoryEntryColumns4());
        return resultCollection.GetCurrent<TeamFoundationJobHistoryEntry>().Items;
      }
    }

    public override List<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryLatestJobHistory");
      this.BindGuid("@jobSource", jobSource);
      this.BindQueryJobsTable("@jobIds", jobIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationJobHistoryEntry>((ObjectBinder<TeamFoundationJobHistoryEntry>) new TeamFoundationJobHistoryEntryColumns4());
        return resultCollection.GetCurrent<TeamFoundationJobHistoryEntry>().Items;
      }
    }

    protected virtual SqlParameter BindJobQueueRemovalTable(
      string parameterName,
      Guid jobSource,
      IEnumerable<Guid> rows)
    {
      rows = rows ?? Enumerable.Empty<Guid>();
      System.Func<Guid, SqlDataRecord> selector = (System.Func<Guid, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent22.typ_JobQueueDeleteTable);
        sqlDataRecord.SetGuid(0, jobSource);
        sqlDataRecord.SetGuid(1, row);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobQueueDeleteTable", rows.Select<Guid, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindJobDefinitionUpdateTable(
      string parameterName,
      Guid jobSource,
      IEnumerable<TeamFoundationJobDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobDefinition>();
      System.Func<TeamFoundationJobDefinition, SqlDataRecord> selector = (System.Func<TeamFoundationJobDefinition, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent22.typ_JobDefinitionUpdateTable4);
        sqlDataRecord.SetGuid(0, jobSource);
        sqlDataRecord.SetGuid(1, row.JobId);
        if (row.PriorityClass == JobPriorityClass.None)
        {
          TeamFoundationTracingService.TraceRaw(2032132, TraceLevel.Error, "Job", nameof (JobQueueComponent22), "jobDefinition.PriorityClass is None");
          row.PriorityClass = JobPriorityClass.Normal;
        }
        sqlDataRecord.SetInt32(2, (int) row.PriorityClass);
        sqlDataRecord.SetBoolean(3, row.IsTemplateJob);
        sqlDataRecord.SetBoolean(4, row.QueueAsDormant);
        DateTime minValue = (DateTime) SqlDateTime.MinValue;
        sqlDataRecord.SetDateTime(5, row.OverrideQueueTime < minValue ? minValue : row.OverrideQueueTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobDefinitionUpdateTable4", rows.Select<TeamFoundationJobDefinition, SqlDataRecord>(selector));
    }

    protected override SqlParameter BindReleaseJobsTable(
      string parameterName,
      IEnumerable<ReleaseJobInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseJobInfo>();
      System.Func<ReleaseJobInfo, SqlDataRecord> selector = (System.Func<ReleaseJobInfo, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent22.typ_ReleaseJobsTable3);
        if (row.JobSource == Guid.Empty)
          TeamFoundationTracingService.TraceRaw(2032133, TraceLevel.Error, "Job", nameof (JobQueueComponent22), "jobToRelease.JobSource is an empty guid.");
        sqlDataRecord.SetGuid(0, row.JobSource);
        sqlDataRecord.SetGuid(1, row.JobId);
        sqlDataRecord.SetByte(2, (byte) row.JobResult);
        if (row.ResultMessage != null)
          sqlDataRecord.SetString(3, row.ResultMessage);
        sqlDataRecord.SetBoolean(4, row.JobDefinitionExists);
        sqlDataRecord.SetInt32(5, (int) row.PriorityClass);
        if (row.ScheduleSeconds > 0)
          sqlDataRecord.SetInt32(6, row.ScheduleSeconds);
        sqlDataRecord.SetInt32(7, row.IgnoreDormancySeconds);
        sqlDataRecord.SetBoolean(8, row.IsTemplateJob);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ReleaseJobsTable3", rows.Select<ReleaseJobInfo, SqlDataRecord>(selector));
    }
  }
}
