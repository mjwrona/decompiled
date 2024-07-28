// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.QuerySqlComponent3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class QuerySqlComponent3 : QuerySqlComponent2
  {
    public override QueryRecordingTableInfo SaveQueryExecutionInformation(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit)
    {
      this.PrepareStoredProcedure("prc_SaveQueryExecutionInformation");
      this.BindQueryExecutionInformationTable("@queryExecutionInformation", queryExecutionInformation);
      this.BindBoolean("@isRecordQueryExecutionDetailsEnabled", true);
      return new QueryRecordingTableInfo()
      {
        InfoTableRowCount = (int) this.ExecuteScalar()
      };
    }

    protected override void BindQueryExecutionInformationTable(
      string parameterName,
      IEnumerable<QueryExecutionInformation> queryExecutionInformation)
    {
      new QuerySqlComponent.QueryExecutionInformationTable2(parameterName, queryExecutionInformation).BindTable((QuerySqlComponent) this);
    }

    public override int CleanupQueryExecutionDetails(
      DateTime cutOffTime,
      int maxRowCount,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupQueryExecutionDetails");
      this.BindDateTime("@cutOffTime", cutOffTime);
      this.BindInt("@maxRowCount", maxRowCount);
      return (int) this.ExecuteScalar();
    }
  }
}
