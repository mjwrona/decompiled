// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.QueuedBuildUpdateTable
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  [Obsolete]
  internal sealed class QueuedBuildUpdateTable : 
    TeamFoundationTableValueParameter<QueuedBuildUpdateOptions>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[6]
    {
      new SqlMetaData("QueueId", SqlDbType.Int),
      new SqlMetaData("Fields", SqlDbType.Int),
      new SqlMetaData("BatchId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Postponed", SqlDbType.Bit),
      new SqlMetaData("Priority", SqlDbType.TinyInt),
      new SqlMetaData("Retry", SqlDbType.TinyInt)
    };

    public QueuedBuildUpdateTable(ICollection<QueuedBuildUpdateOptions> rows)
      : base((IEnumerable<QueuedBuildUpdateOptions>) rows, "typ_QueuedBuildUpdateTableV2", QueuedBuildUpdateTable.s_metadata)
    {
    }

    public override void SetRecord(QueuedBuildUpdateOptions row, SqlDataRecord record)
    {
      record.SetInt32(0, row.QueueId);
      record.SetInt32(1, (int) row.Fields);
      if ((row.Fields & QueuedBuildUpdate.BatchId) == QueuedBuildUpdate.BatchId)
        record.SetGuid(2, row.BatchId);
      else
        record.SetDBNull(2);
      if ((row.Fields & QueuedBuildUpdate.Postponed) == QueuedBuildUpdate.Postponed)
        record.SetBoolean(3, row.Postponed);
      else
        record.SetDBNull(3);
      if ((row.Fields & QueuedBuildUpdate.Priority) == QueuedBuildUpdate.Priority)
        record.SetByte(4, (byte) row.Priority);
      else
        record.SetDBNull(4);
      if (!row.RetryOption.Equals((object) QueuedBuildRetryOption.None) && row.Fields.HasFlag((Enum) QueuedBuildUpdate.Requeue))
        record.SetByte(5, (byte) row.RetryOption);
      else if ((row.Fields & QueuedBuildUpdate.Retry) == QueuedBuildUpdate.Retry && row.Retry)
        record.SetByte(5, (byte) 1);
      else
        record.SetDBNull(5);
    }
  }
}
