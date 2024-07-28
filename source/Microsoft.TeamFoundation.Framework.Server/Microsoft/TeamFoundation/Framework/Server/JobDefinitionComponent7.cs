// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobDefinitionComponent7
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobDefinitionComponent7 : JobDefinitionComponent6
  {
    private static readonly SqlMetaData[] typ_JobScheduleTable2 = new SqlMetaData[5]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ScheduledTime", SqlDbType.DateTime),
      new SqlMetaData("Interval", SqlDbType.Int),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 32L),
      new SqlMetaData("PriorityLevel", SqlDbType.Int)
    };

    protected override SqlParameter BindJobScheduleTable(
      string parameterName,
      IEnumerable<TeamFoundationJobSchedule> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobSchedule>();
      System.Func<TeamFoundationJobSchedule, SqlDataRecord> selector = (System.Func<TeamFoundationJobSchedule, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobDefinitionComponent7.typ_JobScheduleTable2);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetDateTime(1, row.ScheduledTime.ToUniversalTime());
        sqlDataRecord.SetInt32(2, row.Interval);
        sqlDataRecord.SetString(3, row.TimeZoneId);
        JobPriorityLevel jobPriorityLevel = row.PriorityLevel;
        if (jobPriorityLevel == JobPriorityLevel.None)
          jobPriorityLevel = JobPriorityLevel.BelowNormal;
        sqlDataRecord.SetInt32(4, (int) jobPriorityLevel);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobScheduleTable2", rows.Select<TeamFoundationJobSchedule, SqlDataRecord>(selector));
    }
  }
}
