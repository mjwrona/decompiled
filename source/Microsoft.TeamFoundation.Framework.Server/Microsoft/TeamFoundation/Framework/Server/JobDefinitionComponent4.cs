// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobDefinitionComponent4
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

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobDefinitionComponent4 : JobDefinitionComponent3
  {
    private static readonly SqlMetaData[] typ_JobDefinitionTable = new SqlMetaData[7]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("ExtensionName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L),
      new SqlMetaData("EnabledState", SqlDbType.TinyInt),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("PriorityClass", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_JobScheduleTable = new SqlMetaData[6]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ScheduledTime", SqlDbType.DateTime),
      new SqlMetaData("Interval", SqlDbType.Int),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 32L),
      new SqlMetaData("ScheduledTimeDelta", SqlDbType.Int),
      new SqlMetaData("PriorityLevel", SqlDbType.Int)
    };

    public override void DeleteJobs(IEnumerable<Guid> jobIds, bool allowDefinitionUpdates = true)
    {
      this.CheckForDuplicate<Guid>(jobIds, (Exception) new DuplicateJobIdException());
      this.PrepareStoredProcedure("prc_DeleteJobs");
      this.BindSortedGuidTable("@jobIds", jobIds);
      this.BindBoolean("@allowDelete", allowDefinitionUpdates);
      this.ExecuteNonQuery();
    }

    public override void UpdateJobs(
      IEnumerable<TeamFoundationJobDefinition> jobs,
      bool allowDefinitionUpdates = true)
    {
      this.CheckForDuplicate<Guid>(jobs.Select<TeamFoundationJobDefinition, Guid>((System.Func<TeamFoundationJobDefinition, Guid>) (s => s.JobId)), (Exception) new DuplicateJobIdException());
      IEnumerable<TeamFoundationJobSchedule> foundationJobSchedules = jobs.SelectMany<TeamFoundationJobDefinition, TeamFoundationJobSchedule>((System.Func<TeamFoundationJobDefinition, IEnumerable<TeamFoundationJobSchedule>>) (s => (IEnumerable<TeamFoundationJobSchedule>) s.Schedule));
      this.CheckForDuplicate<TeamFoundationJobSchedule>(foundationJobSchedules, (Exception) new DuplicateJobScheduleException());
      this.PrepareStoredProcedure("prc_UpdateJobs");
      this.BindJobDefinitionTable("@definitionUpdates", jobs);
      this.BindJobScheduleTable("@scheduleUpdates", foundationJobSchedules);
      this.BindBoolean("@allowDefinitionUpdates", allowDefinitionUpdates);
      this.ExecuteNonQuery();
    }

    protected void CheckForDuplicate<T>(IEnumerable<T> items, Exception ex)
    {
      List<T> list = items.ToList<T>();
      list.Sort();
      for (int index = 1; index < list.Count; ++index)
      {
        if (Comparer<T>.Default.Compare(list[index], list[index - 1]) == 0)
          throw ex;
      }
    }

    protected override ObjectBinder<TeamFoundationJobDefinition> GetJobDefinitionColumns() => (ObjectBinder<TeamFoundationJobDefinition>) new JobDefinitionComponent2.TeamFoundationJobDefinitionColumns2();

    protected SqlParameter BindJobDefinitionTable(
      string parameterName,
      IEnumerable<TeamFoundationJobDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobDefinition>();
      System.Func<TeamFoundationJobDefinition, SqlDataRecord> selector = (System.Func<TeamFoundationJobDefinition, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobDefinitionComponent4.typ_JobDefinitionTable);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetString(1, row.Name);
        sqlDataRecord.SetString(2, row.ExtensionName);
        if (row.Data != null)
          sqlDataRecord.SetString(3, row.Data.OuterXml);
        else
          sqlDataRecord.SetDBNull(3);
        sqlDataRecord.SetByte(4, (byte) row.EnabledState);
        sqlDataRecord.SetInt32(5, (int) row.Flags);
        JobPriorityClass jobPriorityClass = row.PriorityClass;
        if (jobPriorityClass == JobPriorityClass.None)
          jobPriorityClass = JobPriorityClass.Normal;
        sqlDataRecord.SetInt32(6, (int) jobPriorityClass);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobDefinitionTable", rows.Select<TeamFoundationJobDefinition, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindJobScheduleTable(
      string parameterName,
      IEnumerable<TeamFoundationJobSchedule> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobSchedule>();
      System.Func<TeamFoundationJobSchedule, SqlDataRecord> selector = (System.Func<TeamFoundationJobSchedule, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobDefinitionComponent4.typ_JobScheduleTable);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetDateTime(1, row.ScheduledTime.ToUniversalTime());
        sqlDataRecord.SetInt32(2, row.Interval);
        sqlDataRecord.SetString(3, row.TimeZoneId);
        sqlDataRecord.SetInt32(4, Convert.ToInt32(JobComponentBase.GetScheduledTimeDelta(row).TotalSeconds));
        JobPriorityLevel jobPriorityLevel = row.PriorityLevel;
        if (jobPriorityLevel == JobPriorityLevel.None)
          jobPriorityLevel = JobPriorityLevel.BelowNormal;
        sqlDataRecord.SetInt32(5, (int) jobPriorityLevel);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobScheduleTable", rows.Select<TeamFoundationJobSchedule, SqlDataRecord>(selector));
    }
  }
}
