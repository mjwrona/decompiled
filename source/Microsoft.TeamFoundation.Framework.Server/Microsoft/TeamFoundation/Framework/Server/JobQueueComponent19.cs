// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent19
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
  internal class JobQueueComponent19 : JobQueueComponent18
  {
    private static readonly SqlMetaData[] typ_JobQueueUpdateTable = new SqlMetaData[2]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Priority", SqlDbType.TinyInt)
    };

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
      this.BindJobQueueUpdateTable("@jobList", jobsToRun);
      this.BindInt("@priorityLevel", (int) priorityLevel);
      this.BindInt("@delaySeconds", delaySeconds);
      this.BindBoolean("@queueAsDormant", queueAsDormant);
      return (int) this.ExecuteScalar();
    }

    internal SqlParameter BindJobQueueUpdateTable(
      string parameterName,
      IEnumerable<Tuple<Guid, int>> rows)
    {
      rows = rows ?? Enumerable.Empty<Tuple<Guid, int>>();
      System.Func<Tuple<Guid, int>, SqlDataRecord> selector = (System.Func<Tuple<Guid, int>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent19.typ_JobQueueUpdateTable);
        sqlDataRecord.SetGuid(0, row.Item1);
        sqlDataRecord.SetByte(1, (byte) row.Item2);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobQueueUpdateTable", rows.Select<Tuple<Guid, int>, SqlDataRecord>(selector));
    }
  }
}
