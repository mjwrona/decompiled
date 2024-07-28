// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobDefinitionComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobDefinitionComponent : JobComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[8]
    {
      (IComponentCreator) new ComponentCreator<JobDefinitionComponent>(1),
      (IComponentCreator) new ComponentCreator<JobDefinitionComponent2>(2),
      (IComponentCreator) new ComponentCreator<JobDefinitionComponent3>(3, true),
      (IComponentCreator) new ComponentCreator<JobDefinitionComponent4>(4),
      (IComponentCreator) new ComponentCreator<JobDefinitionComponent5>(5),
      (IComponentCreator) new ComponentCreator<JobDefinitionComponent6>(6),
      (IComponentCreator) new ComponentCreator<JobDefinitionComponent7>(7),
      (IComponentCreator) new ComponentCreator<JobDefinitionComponent8>(8)
    }, "JobDefinition");
    private static readonly SqlMetaData[] typ_JobUpdateTable = new SqlMetaData[12]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("ExtensionName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L),
      new SqlMetaData("EnabledState", SqlDbType.TinyInt),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("LastExecutionTime", SqlDbType.DateTime2),
      new SqlMetaData("JobScheduleUpdateId", SqlDbType.Int),
      new SqlMetaData("ScheduledTime", SqlDbType.DateTime2),
      new SqlMetaData("Interval", SqlDbType.Int),
      new SqlMetaData("ScheduledTimeDelta", SqlDbType.Int),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 32L)
    };

    public virtual void DeleteJobs(IEnumerable<Guid> jobIds, bool allowDefinitionUpdates = true)
    {
      this.PrepareStoredProcedure("prc_UpdateJobs");
      this.BindGuidTable("@jobRemovals", jobIds);
      this.BindJobUpdateTable("@jobUpdates", Enumerable.Empty<TeamFoundationJobDefinition>());
      this.ExecuteNonQuery();
    }

    public virtual void UpdateJobs(
      IEnumerable<TeamFoundationJobDefinition> jobs,
      bool allowDefinitionUpdates = true)
    {
      this.PrepareStoredProcedure("prc_UpdateJobs");
      this.BindGuidTable("@jobRemovals", Enumerable.Empty<Guid>());
      this.BindJobUpdateTable("@jobUpdates", jobs);
      this.ExecuteNonQuery();
    }

    public virtual List<TeamFoundationJobDefinition> QueryJobs(IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryJobs", 3600);
      this.BindQueryJobsTable("@jobIds", jobIds);
      ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      rc.AddBinder<TeamFoundationJobDefinition>(this.GetJobDefinitionColumns());
      rc.AddBinder<TeamFoundationJobSchedule>((ObjectBinder<TeamFoundationJobSchedule>) this.GetJobScheduleColumns());
      return this.PopulateJobs(rc);
    }

    public virtual TeamFoundationJobDefinition QueryJob(Guid jobId)
    {
      this.PrepareStoredProcedure("prc_QueryJob");
      this.BindGuid("@jobId", jobId);
      ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      rc.AddBinder<TeamFoundationJobDefinition>(this.GetJobDefinitionColumns());
      rc.AddBinder<TeamFoundationJobSchedule>((ObjectBinder<TeamFoundationJobSchedule>) this.GetJobScheduleColumns());
      return this.PopulateJobs(rc).SingleOrDefault<TeamFoundationJobDefinition>();
    }

    public virtual void UpdateLastExecutionTime(Guid jobId)
    {
      this.PrepareStoredProcedure("prc_UpdateLastExecutionTime");
      this.BindGuid("@jobId", jobId);
      this.ExecuteNonQuery();
    }

    public virtual int DeleteOneTimeJobs(DateTime? completedTo = null, int batchSize = 0) => 0;

    protected virtual SqlParameter BindJobUpdateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobDefinition>();
      return this.BindTable(parameterName, "typ_JobUpdateTable", rows.SelectMany<TeamFoundationJobDefinition, SqlDataRecord>(new System.Func<TeamFoundationJobDefinition, IEnumerable<SqlDataRecord>>(this.BindJobUpdateRows)));
    }

    protected List<TeamFoundationJobDefinition> PopulateJobs(ResultCollection rc)
    {
      Dictionary<Guid, TeamFoundationJobDefinition> dictionary = new Dictionary<Guid, TeamFoundationJobDefinition>();
      List<TeamFoundationJobDefinition> items = rc.GetCurrent<TeamFoundationJobDefinition>().Items;
      foreach (TeamFoundationJobDefinition foundationJobDefinition in items)
      {
        if (foundationJobDefinition != null)
        {
          foundationJobDefinition.Queried = true;
          dictionary[foundationJobDefinition.JobId] = foundationJobDefinition;
        }
      }
      if (rc.TryNextResult())
      {
        foreach (TeamFoundationJobSchedule foundationJobSchedule in rc.GetCurrent<TeamFoundationJobSchedule>().Items)
        {
          TeamFoundationJobDefinition foundationJobDefinition;
          if (dictionary.TryGetValue(foundationJobSchedule.JobId, out foundationJobDefinition))
            foundationJobDefinition.Schedule.Add(foundationJobSchedule);
        }
      }
      return items;
    }

    private IEnumerable<SqlDataRecord> BindJobUpdateRows(TeamFoundationJobDefinition job)
    {
      Func<SqlDataRecord> createRecord = (Func<SqlDataRecord>) (() =>
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(JobDefinitionComponent.typ_JobUpdateTable);
        sqlDataRecord1.SetGuid(0, job.JobId);
        sqlDataRecord1.SetString(1, job.Name);
        sqlDataRecord1.SetString(2, job.ExtensionName);
        if (job.Data != null)
          sqlDataRecord1.SetString(3, job.Data.OuterXml);
        else
          sqlDataRecord1.SetDBNull(3);
        sqlDataRecord1.SetByte(4, (byte) job.EnabledState);
        sqlDataRecord1.SetInt32(5, (int) job.Flags);
        DateTime? lastExecutionTime = job.LastExecutionTime;
        if (lastExecutionTime.HasValue)
        {
          SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
          lastExecutionTime = job.LastExecutionTime;
          DateTime dateTime = lastExecutionTime.Value;
          sqlDataRecord2.SetDateTime(6, dateTime);
        }
        return sqlDataRecord1;
      });
      if (job.Schedule != null && job.Schedule.Count > 0)
      {
        int index = 0;
        foreach (TeamFoundationJobSchedule jobSchedule in job.Schedule)
        {
          SqlDataRecord sqlDataRecord = createRecord();
          sqlDataRecord.SetInt32(7, index++);
          sqlDataRecord.SetDateTime(8, jobSchedule.ScheduledTime.ToUniversalTime());
          sqlDataRecord.SetInt32(9, jobSchedule.Interval);
          sqlDataRecord.SetInt32(10, Convert.ToInt32(JobComponentBase.GetScheduledTimeDelta(jobSchedule).TotalSeconds));
          sqlDataRecord.SetString(11, jobSchedule.TimeZoneId);
          yield return sqlDataRecord;
        }
      }
      else
      {
        SqlDataRecord sqlDataRecord = createRecord();
        sqlDataRecord.SetInt32(7, 0);
        sqlDataRecord.SetDBNull(8);
        sqlDataRecord.SetDBNull(9);
        sqlDataRecord.SetDBNull(10);
        sqlDataRecord.SetDBNull(11);
        yield return sqlDataRecord;
      }
    }

    protected virtual ObjectBinder<TeamFoundationJobDefinition> GetJobDefinitionColumns() => (ObjectBinder<TeamFoundationJobDefinition>) new JobDefinitionComponent.TeamFoundationJobDefinitionColumns();

    protected virtual JobDefinitionComponent.TeamFoundationJobScheduleColumns GetJobScheduleColumns() => new JobDefinitionComponent.TeamFoundationJobScheduleColumns();

    protected class TeamFoundationJobDefinitionColumns : ObjectBinder<TeamFoundationJobDefinition>
    {
      private SqlColumnBinder JobIdColumn = new SqlColumnBinder("JobId");
      private SqlColumnBinder JobNameColumn = new SqlColumnBinder("JobName");
      private SqlColumnBinder JobExtensionNameColumn = new SqlColumnBinder("ExtensionName");
      private SqlColumnBinder JobDataColumn = new SqlColumnBinder("Data");
      private SqlColumnBinder JobEnabledStateColumn = new SqlColumnBinder("EnabledState");
      private SqlColumnBinder JobFlagsColumn = new SqlColumnBinder("Flags");
      private SqlColumnBinder JobLastExecutionTimeColumn = new SqlColumnBinder("LastExecutionTime");

      protected override TeamFoundationJobDefinition Bind()
      {
        if (this.JobIdColumn.IsNull((IDataReader) this.Reader))
          return (TeamFoundationJobDefinition) null;
        XmlNode jobDataNode = TeamFoundationJobDefinition.StringToJobDataNode(this.JobDataColumn.GetString((IDataReader) this.Reader, true));
        return new TeamFoundationJobDefinition()
        {
          JobId = this.JobIdColumn.GetGuid((IDataReader) this.Reader),
          Name = this.JobNameColumn.GetString((IDataReader) this.Reader, false),
          ExtensionName = this.JobExtensionNameColumn.GetString((IDataReader) this.Reader, false),
          Data = jobDataNode,
          EnabledState = (TeamFoundationJobEnabledState) this.JobEnabledStateColumn.GetByte((IDataReader) this.Reader),
          Flags = (TeamFoundationJobDefinitionFlags) this.JobFlagsColumn.GetInt32((IDataReader) this.Reader),
          LastExecutionTime = this.JobLastExecutionTimeColumn.IsNull((IDataReader) this.Reader) ? new DateTime?() : new DateTime?(this.JobLastExecutionTimeColumn.GetDateTime((IDataReader) this.Reader))
        };
      }
    }

    protected class TeamFoundationJobScheduleColumns : ObjectBinder<TeamFoundationJobSchedule>
    {
      private SqlColumnBinder JobIdColumn = new SqlColumnBinder("JobId");
      private SqlColumnBinder ScheduledTimeColumn = new SqlColumnBinder("ScheduledTime");
      private SqlColumnBinder IntervalColumn = new SqlColumnBinder("Interval");
      private SqlColumnBinder TimeZoneId = new SqlColumnBinder(nameof (TimeZoneId));

      protected override TeamFoundationJobSchedule Bind() => new TeamFoundationJobSchedule()
      {
        JobId = this.JobIdColumn.GetGuid((IDataReader) this.Reader),
        ScheduledTime = this.ScheduledTimeColumn.GetDateTime((IDataReader) this.Reader),
        Interval = this.IntervalColumn.GetInt32((IDataReader) this.Reader),
        TimeZoneId = this.TimeZoneId.GetString((IDataReader) this.Reader, false)
      };
    }
  }
}
