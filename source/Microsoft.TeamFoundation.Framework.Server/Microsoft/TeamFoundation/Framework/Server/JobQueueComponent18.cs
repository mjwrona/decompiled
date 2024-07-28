// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent18
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent18 : JobQueueComponent17
  {
    private static readonly SqlMetaData[] typ_DaylightTransitionInfoTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 260L),
      new SqlMetaData("StartTime", SqlDbType.DateTime2),
      new SqlMetaData("EndTime", SqlDbType.DateTime2),
      new SqlMetaData("TimeDelta", SqlDbType.Float)
    };

    internal override List<TeamFoundationJobQueueEntry> RescheduleJobs(
      IEnumerable<Guid> onlineProcesses)
    {
      this.PrepareStoredProcedure("prc_RescheduleJobs");
      this.BindSortedGuidTable("@onlineProcesses", onlineProcesses);
      this.ExecuteNonQuery();
      return new List<TeamFoundationJobQueueEntry>();
    }

    protected override SqlParameter BindJobScheduleUpdateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobSchedule> rows)
    {
      return this.BindJobScheduleUpdateTableForReleaseJobs(parameterName, rows);
    }

    protected override SqlParameter BindDaylightTransitionInfoTable(
      string parameterName,
      IEnumerable<DaylightTransitionInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<DaylightTransitionInfo>();
      System.Func<DaylightTransitionInfo, SqlDataRecord> selector = (System.Func<DaylightTransitionInfo, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent18.typ_DaylightTransitionInfoTable2);
        sqlDataRecord.SetString(0, row.TimeZoneId);
        sqlDataRecord.SetDateTime(1, row.StartDate.ToUniversalTime());
        sqlDataRecord.SetDateTime(2, row.EndDate.ToUniversalTime());
        sqlDataRecord.SetDouble(3, row.Delta.TotalSeconds);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_DaylightTransitionInfoTable2", rows.Select<DaylightTransitionInfo, SqlDataRecord>(selector));
    }
  }
}
