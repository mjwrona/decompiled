// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobDefinitionComponent2
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
  internal class JobDefinitionComponent2 : JobDefinitionComponent
  {
    private static readonly SqlMetaData[] typ_JobUpdateTable2 = new SqlMetaData[14]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("ExtensionName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L),
      new SqlMetaData("EnabledState", SqlDbType.TinyInt),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("LastExecutionTime", SqlDbType.DateTime2),
      new SqlMetaData("PriorityClass", SqlDbType.Int),
      new SqlMetaData("JobScheduleUpdateId", SqlDbType.Int),
      new SqlMetaData("ScheduledTime", SqlDbType.DateTime2),
      new SqlMetaData("Interval", SqlDbType.Int),
      new SqlMetaData("ScheduledTimeDelta", SqlDbType.Int),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 32L),
      new SqlMetaData("PriorityLevel", SqlDbType.Int)
    };

    protected override SqlParameter BindJobUpdateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobDefinition>();
      return this.BindTable(parameterName, "typ_JobUpdateTable2", rows.SelectMany<TeamFoundationJobDefinition, SqlDataRecord>(new System.Func<TeamFoundationJobDefinition, IEnumerable<SqlDataRecord>>(this.BindJobUpdate2Rows)));
    }

    private IEnumerable<SqlDataRecord> BindJobUpdate2Rows(TeamFoundationJobDefinition job)
    {
      Func<SqlDataRecord> createRecord = (Func<SqlDataRecord>) (() =>
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(JobDefinitionComponent2.typ_JobUpdateTable2);
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
        JobPriorityClass jobPriorityClass = job.PriorityClass;
        if (jobPriorityClass == JobPriorityClass.None)
          jobPriorityClass = JobPriorityClass.Normal;
        sqlDataRecord1.SetInt32(7, (int) jobPriorityClass);
        return sqlDataRecord1;
      });
      if (job.Schedule != null && job.Schedule.Count > 0)
      {
        int index = 0;
        foreach (TeamFoundationJobSchedule jobSchedule in job.Schedule)
        {
          SqlDataRecord sqlDataRecord = createRecord();
          sqlDataRecord.SetInt32(8, index++);
          sqlDataRecord.SetDateTime(9, jobSchedule.ScheduledTime.ToUniversalTime());
          sqlDataRecord.SetInt32(10, jobSchedule.Interval);
          sqlDataRecord.SetInt32(11, Convert.ToInt32(JobComponentBase.GetScheduledTimeDelta(jobSchedule).TotalSeconds));
          sqlDataRecord.SetString(12, jobSchedule.TimeZoneId);
          JobPriorityLevel jobPriorityLevel = jobSchedule.PriorityLevel;
          if (jobPriorityLevel == JobPriorityLevel.None)
            jobPriorityLevel = JobPriorityLevel.BelowNormal;
          sqlDataRecord.SetInt32(13, (int) jobPriorityLevel);
          yield return sqlDataRecord;
        }
      }
      else
      {
        SqlDataRecord sqlDataRecord = createRecord();
        sqlDataRecord.SetInt32(8, 0);
        sqlDataRecord.SetDBNull(9);
        sqlDataRecord.SetDBNull(10);
        sqlDataRecord.SetDBNull(11);
        sqlDataRecord.SetDBNull(12);
        sqlDataRecord.SetDBNull(13);
        yield return sqlDataRecord;
      }
    }

    protected class TeamFoundationJobDefinitionColumns2 : 
      JobDefinitionComponent.TeamFoundationJobDefinitionColumns
    {
      private SqlColumnBinder PriorityClassColumn = new SqlColumnBinder("PriorityClass");

      protected override TeamFoundationJobDefinition Bind()
      {
        TeamFoundationJobDefinition foundationJobDefinition = base.Bind();
        if (foundationJobDefinition != null)
          foundationJobDefinition.PriorityClass = (JobPriorityClass) this.PriorityClassColumn.GetInt32((IDataReader) this.Reader);
        return foundationJobDefinition;
      }
    }

    protected class TeamFoundationJobScheduleColumns2 : 
      JobDefinitionComponent.TeamFoundationJobScheduleColumns
    {
      private SqlColumnBinder PriorityLevelColumn = new SqlColumnBinder("PriorityLevel");

      protected override TeamFoundationJobSchedule Bind()
      {
        TeamFoundationJobSchedule foundationJobSchedule = base.Bind();
        foundationJobSchedule.PriorityLevel = (JobPriorityLevel) this.PriorityLevelColumn.GetInt32((IDataReader) this.Reader);
        return foundationJobSchedule;
      }
    }
  }
}
