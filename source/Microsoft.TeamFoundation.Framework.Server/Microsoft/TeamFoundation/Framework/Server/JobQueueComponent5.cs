// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent5 : JobQueueComponent4
  {
    private static readonly SqlMetaData[] typ_ReleaseJobsTable2 = new SqlMetaData[8]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobResult", SqlDbType.TinyInt),
      new SqlMetaData("JobResultMessage", SqlDbType.NVarChar, -1L),
      new SqlMetaData("JobDefinitionExists", SqlDbType.Bit),
      new SqlMetaData("PriorityClass", SqlDbType.Int),
      new SqlMetaData("ScheduleSeconds", SqlDbType.Int),
      new SqlMetaData("IgnoreDormancySeconds", SqlDbType.Int)
    };

    internal override List<TeamFoundationJobQueueEntry> RescheduleJobs(
      IEnumerable<Guid> onlineProcesses)
    {
      this.PrepareStoredProcedure("prc_RescheduleJobs");
      this.BindGuidTable("@onlineProcesses", onlineProcesses);
      this.ExecuteNonQuery();
      return new List<TeamFoundationJobQueueEntry>();
    }

    protected override SqlParameter BindReleaseJobsTable(
      string parameterName,
      IEnumerable<ReleaseJobInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseJobInfo>();
      System.Func<ReleaseJobInfo, SqlDataRecord> selector = (System.Func<ReleaseJobInfo, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent5.typ_ReleaseJobsTable2);
        if (row.JobSource == Guid.Empty)
          TeamFoundationTracingService.TraceRaw(2032128, TraceLevel.Error, "Job", nameof (JobQueueComponent5), "jobToRelease.JobSource is an empty guid.");
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
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ReleaseJobsTable2", rows.Select<ReleaseJobInfo, SqlDataRecord>(selector));
    }
  }
}
