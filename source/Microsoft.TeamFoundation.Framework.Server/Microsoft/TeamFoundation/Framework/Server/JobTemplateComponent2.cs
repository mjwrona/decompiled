// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobTemplateComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobTemplateComponent2 : JobTemplateComponent
  {
    private static readonly SqlMetaData[] typ_JobDefinitionTemplateTable = new SqlMetaData[9]
    {
      new SqlMetaData("HostType", SqlDbType.Int),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("PluginType", SqlDbType.NVarChar, 128L),
      new SqlMetaData("JobData", SqlDbType.NVarChar, -1L),
      new SqlMetaData("EnabledState", SqlDbType.TinyInt),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("PriorityClass", SqlDbType.Int),
      new SqlMetaData("InitialStagger", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_JobScheduleTable = new SqlMetaData[6]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BaseTime", SqlDbType.DateTime),
      new SqlMetaData("StaggerInterval", SqlDbType.Int),
      new SqlMetaData("ScheduleInterval", SqlDbType.Int),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 32L),
      new SqlMetaData("PriorityLevel", SqlDbType.Int)
    };

    public override IList<TeamFoundationJobDefinitionTemplate> QueryJobTemplates(
      bool includeDeleted,
      out long sequenceId)
    {
      this.PrepareStoredProcedure("JobService.prc_QueryJobTemplates");
      this.BindBoolean("@includeDeleted", includeDeleted);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<long>((ObjectBinder<long>) new JobTemplateComponent2.SequenceIdColumn());
        sequenceId = rc.GetCurrent<long>().Single<long>();
        rc.NextResult();
        return this.ReadJobTemplates(rc);
      }
    }

    public override TeamFoundationJobDefinitionTemplate QueryJobTemplate(
      Guid jobId,
      bool includeDeleted,
      out long sequenceId)
    {
      this.PrepareStoredProcedure("JobService.prc_QueryJobTemplate");
      this.BindGuid("@jobId", jobId);
      this.BindBoolean("@includeDeleted", includeDeleted);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<long>((ObjectBinder<long>) new JobTemplateComponent2.SequenceIdColumn());
        sequenceId = rc.GetCurrent<long>().Single<long>();
        rc.NextResult();
        return this.ReadJobTemplates(rc).SingleOrDefault<TeamFoundationJobDefinitionTemplate>();
      }
    }

    public override void UpdateJobTemplates(
      IEnumerable<TeamFoundationJobDefinitionTemplate> jobTemplates)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TeamFoundationJobDefinitionTemplate>>(jobTemplates, nameof (jobTemplates));
      this.PrepareStoredProcedure("JobService.prc_UpdateJobTemplates");
      this.BindJobDefinitionTemplateTable("@definitionUpdates", jobTemplates);
      this.BindJobScheduleTemplateTable("@scheduleUpdates", jobTemplates);
      this.ExecuteNonQuery();
    }

    public override void DeleteJobTemplates(IEnumerable<Guid> jobIds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(jobIds, nameof (jobIds));
      this.PrepareStoredProcedure("JobService.prc_DeleteJobTemplates");
      this.BindSortedGuidTable("@jobIds", jobIds);
      this.ExecuteNonQuery();
    }

    public override void PurgeDeletedJobTemplates()
    {
      this.PrepareStoredProcedure("JobService.prc_PurgeJobTemplates");
      this.ExecuteNonQuery();
    }

    public override Guid StaggerPendingJobTemplates(
      Guid hostIdWatermark = default (Guid),
      long maxSequenceId = 9223372036854775807,
      int batchSize = 1000,
      int maxBatches = 2147483647,
      TimeSpan? timeout = null)
    {
      int commandTimeout = 3600;
      if (timeout.HasValue)
        commandTimeout = (int) timeout.Value.TotalSeconds;
      this.PrepareStoredProcedure("JobService.prc_StaggerJobTemplates", commandTimeout);
      this.ExecuteNonQuery();
      return Guid.Empty;
    }

    public override void StaggerJobTemplates(Guid hostId)
    {
      this.PrepareStoredProcedure("JobService.prc_StaggerJobTemplatesForOneHost");
      this.BindGuid("@jobSource", hostId);
      this.ExecuteNonQuery();
    }

    private IList<TeamFoundationJobDefinitionTemplate> ReadJobTemplates(ResultCollection rc)
    {
      rc.AddBinder<TeamFoundationJobDefinitionTemplate>((ObjectBinder<TeamFoundationJobDefinitionTemplate>) new JobTemplateComponent2.TeamFoundationJobDefinitionTemplateColumns());
      rc.AddBinder<KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>>((ObjectBinder<KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>>) new JobTemplateComponent2.TeamFoundationJobScheduleTemplateColumns());
      List<TeamFoundationJobDefinitionTemplate> items = rc.GetCurrent<TeamFoundationJobDefinitionTemplate>().Items;
      if (rc.TryNextResult())
      {
        Dictionary<Guid, TeamFoundationJobDefinitionTemplate> dictionary = items.ToDictionary<TeamFoundationJobDefinitionTemplate, Guid>((System.Func<TeamFoundationJobDefinitionTemplate, Guid>) (x => x.JobId));
        foreach (KeyValuePair<Guid, TeamFoundationJobScheduleTemplate> keyValuePair in rc.GetCurrent<KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>>().Items)
        {
          TeamFoundationJobDefinitionTemplate definitionTemplate;
          if (dictionary.TryGetValue(keyValuePair.Key, out definitionTemplate))
            definitionTemplate.Schedules.Add(keyValuePair.Value);
        }
      }
      return (IList<TeamFoundationJobDefinitionTemplate>) items;
    }

    private SqlParameter BindJobDefinitionTemplateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobDefinitionTemplate> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobDefinitionTemplate>();
      System.Func<TeamFoundationJobDefinitionTemplate, SqlDataRecord> selector = (System.Func<TeamFoundationJobDefinitionTemplate, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobTemplateComponent2.typ_JobDefinitionTemplateTable);
        sqlDataRecord.SetInt32(0, (int) row.HostType);
        sqlDataRecord.SetGuid(1, row.JobId);
        sqlDataRecord.SetString(2, row.JobName);
        sqlDataRecord.SetString(3, row.PluginType);
        if (row.JobData != null)
          sqlDataRecord.SetString(4, row.JobData.OuterXml);
        else
          sqlDataRecord.SetDBNull(4);
        sqlDataRecord.SetByte(5, (byte) row.EnabledState);
        sqlDataRecord.SetInt32(6, (int) row.Flags);
        sqlDataRecord.SetInt32(7, (int) row.PriorityClass);
        sqlDataRecord.SetInt32(8, Convert.ToInt32(row.InitialStagger.TotalSeconds));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "JobService.typ_JobDefinitionTemplate", rows.Select<TeamFoundationJobDefinitionTemplate, SqlDataRecord>(selector));
    }

    private SqlParameter BindJobScheduleTemplateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobDefinitionTemplate> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobDefinitionTemplate>();
      System.Func<KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>, SqlDataRecord> selector = (System.Func<KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobTemplateComponent2.typ_JobScheduleTable);
        sqlDataRecord.SetGuid(0, row.Key);
        sqlDataRecord.SetDateTime(1, row.Value.BaseTime.ToUniversalTime());
        sqlDataRecord.SetInt32(2, Convert.ToInt32(row.Value.StaggerInterval.TotalSeconds));
        sqlDataRecord.SetInt32(3, Convert.ToInt32(row.Value.ScheduleInterval.TotalSeconds));
        sqlDataRecord.SetString(4, row.Value.TimeZoneId);
        sqlDataRecord.SetInt32(5, (int) row.Value.PriorityLevel);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "JobService.typ_JobScheduleTemplate", JobTemplateComponent2.EnumerateSchedules(rows).Select<KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>, SqlDataRecord>(selector));
    }

    private static IEnumerable<KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>> EnumerateSchedules(
      IEnumerable<TeamFoundationJobDefinitionTemplate> rows)
    {
      foreach (TeamFoundationJobDefinitionTemplate definitionTemplate in rows)
      {
        foreach (TeamFoundationJobScheduleTemplate schedule in definitionTemplate.Schedules)
          yield return new KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>(definitionTemplate.JobId, schedule);
      }
    }

    private class TeamFoundationJobDefinitionTemplateColumns : 
      ObjectBinder<TeamFoundationJobDefinitionTemplate>
    {
      private SqlColumnBinder HostTypeColumn = new SqlColumnBinder("HostType");
      private SqlColumnBinder JobIdColumn = new SqlColumnBinder("JobId");
      private SqlColumnBinder JobNameColumn = new SqlColumnBinder("JobName");
      private SqlColumnBinder JobPluginTypeColumn = new SqlColumnBinder("PluginType");
      private SqlColumnBinder JobDataColumn = new SqlColumnBinder("JobData");
      private SqlColumnBinder JobEnabledStateColumn = new SqlColumnBinder("EnabledState");
      private SqlColumnBinder JobFlagsColumn = new SqlColumnBinder("Flags");
      private SqlColumnBinder PriorityClassColumn = new SqlColumnBinder("PriorityClass");
      private SqlColumnBinder InitialStaggerColumn = new SqlColumnBinder("InitialStagger");
      private SqlColumnBinder PendingStaggeringColumn = new SqlColumnBinder("PendingStaggering");

      protected override TeamFoundationJobDefinitionTemplate Bind() => new TeamFoundationJobDefinitionTemplate()
      {
        HostType = (TeamFoundationHostType) this.HostTypeColumn.GetInt32((IDataReader) this.Reader),
        JobId = this.JobIdColumn.GetGuid((IDataReader) this.Reader),
        JobName = this.JobNameColumn.GetString((IDataReader) this.Reader, false),
        PluginType = this.JobPluginTypeColumn.GetString((IDataReader) this.Reader, false),
        JobData = TeamFoundationJobDefinition.StringToJobDataNode(this.JobDataColumn.GetString((IDataReader) this.Reader, true)),
        EnabledState = (TeamFoundationJobEnabledState) this.JobEnabledStateColumn.GetByte((IDataReader) this.Reader),
        Flags = (TeamFoundationJobDefinitionTemplateFlags) this.JobFlagsColumn.GetInt32((IDataReader) this.Reader),
        PriorityClass = (JobPriorityClass) this.PriorityClassColumn.GetInt32((IDataReader) this.Reader),
        InitialStagger = TimeSpan.FromSeconds((double) this.InitialStaggerColumn.GetInt32((IDataReader) this.Reader)),
        PendingStaggering = this.PendingStaggeringColumn.GetBoolean((IDataReader) this.Reader)
      };
    }

    private class TeamFoundationJobScheduleTemplateColumns : 
      ObjectBinder<KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>>
    {
      private SqlColumnBinder JobIdColumn = new SqlColumnBinder("JobId");
      private SqlColumnBinder BaseTimeColumn = new SqlColumnBinder("BaseTime");
      private SqlColumnBinder StaggerIntervalColumn = new SqlColumnBinder("StaggerInterval");
      private SqlColumnBinder ScheduleIntervalColumn = new SqlColumnBinder("ScheduleInterval");
      private SqlColumnBinder TimeZoneIdColumn = new SqlColumnBinder("TimeZoneId");
      private SqlColumnBinder PriorityLevelColumn = new SqlColumnBinder("PriorityLevel");

      protected override KeyValuePair<Guid, TeamFoundationJobScheduleTemplate> Bind() => new KeyValuePair<Guid, TeamFoundationJobScheduleTemplate>(this.JobIdColumn.GetGuid((IDataReader) this.Reader), new TeamFoundationJobScheduleTemplate()
      {
        BaseTime = this.BaseTimeColumn.GetDateTime((IDataReader) this.Reader),
        StaggerInterval = TimeSpan.FromSeconds((double) this.StaggerIntervalColumn.GetInt32((IDataReader) this.Reader)),
        ScheduleInterval = TimeSpan.FromSeconds((double) this.ScheduleIntervalColumn.GetInt32((IDataReader) this.Reader)),
        TimeZoneId = this.TimeZoneIdColumn.GetString((IDataReader) this.Reader, false),
        PriorityLevel = (JobPriorityLevel) this.PriorityLevelColumn.GetInt32((IDataReader) this.Reader)
      });
    }

    private class SequenceIdColumn : ObjectBinder<long>
    {
      private SqlColumnBinder m_sequenceIdColumn = new SqlColumnBinder("SequenceId");

      protected override long Bind() => this.m_sequenceIdColumn.GetInt64((IDataReader) this.Reader);
    }
  }
}
