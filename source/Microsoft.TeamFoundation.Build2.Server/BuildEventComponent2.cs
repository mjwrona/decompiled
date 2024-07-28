// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildEventComponent2
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class BuildEventComponent2 : BuildEventComponent
  {
    protected static readonly SqlMetaData[] typ_BuildEventStatusUpdateTable2 = new SqlMetaData[3]
    {
      new SqlMetaData("PartitionId", SqlDbType.Int),
      new SqlMetaData("BuildEventId", SqlDbType.BigInt),
      new SqlMetaData("Status", SqlDbType.TinyInt)
    };

    protected override SqlParameter BindBuildEventStatusUpdateTable(
      string parameterName,
      IEnumerable<BuildChangeEvent> buildEventStatusUpdates)
    {
      buildEventStatusUpdates = buildEventStatusUpdates ?? Enumerable.Empty<BuildChangeEvent>();
      System.Func<BuildChangeEvent, SqlDataRecord> selector = (System.Func<BuildChangeEvent, SqlDataRecord>) (buildEventStatusUpdate =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildEventComponent2.typ_BuildEventStatusUpdateTable2);
        sqlDataRecord.SetInt32(0, this.PartitionId);
        sqlDataRecord.SetInt64(1, buildEventStatusUpdate.BuildEventId);
        sqlDataRecord.SetByte(2, (byte) buildEventStatusUpdate.Status);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Build.typ_BuildEventStatusUpdateTable2", buildEventStatusUpdates.Select<BuildChangeEvent, SqlDataRecord>(selector));
    }
  }
}
