// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent23
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent23 : JobQueueComponent22
  {
    private static readonly SqlMetaData[] typ_JobQueueEntryWithJobSourceAndPriorityTable = new SqlMetaData[3]
    {
      new SqlMetaData("JobSource", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Priority", SqlDbType.TinyInt)
    };

    internal override int UpdateJobQueuePriorityDirect(
      IEnumerable<TeamFoundationJobQueueEntry> jobQueueUpdates)
    {
      this.PrepareStoredProcedure("prc_UpdateJobQueuePriorityDirect");
      this.BindJobQueueEntryWithJobSourceAndPriorityTable("@jobQueueUpdates", jobQueueUpdates);
      return (int) this.ExecuteScalar();
    }

    protected virtual SqlParameter BindJobQueueEntryWithJobSourceAndPriorityTable(
      string parameterName,
      IEnumerable<TeamFoundationJobQueueEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobQueueEntry>();
      System.Func<TeamFoundationJobQueueEntry, SqlDataRecord> selector = (System.Func<TeamFoundationJobQueueEntry, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent23.typ_JobQueueEntryWithJobSourceAndPriorityTable);
        sqlDataRecord.SetGuid(0, row.JobSource);
        sqlDataRecord.SetGuid(1, row.JobId);
        sqlDataRecord.SetByte(2, (byte) row.Priority);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobQueueEntryWithJobSourceAndPriorityTable", rows.Select<TeamFoundationJobQueueEntry, SqlDataRecord>(selector));
    }
  }
}
