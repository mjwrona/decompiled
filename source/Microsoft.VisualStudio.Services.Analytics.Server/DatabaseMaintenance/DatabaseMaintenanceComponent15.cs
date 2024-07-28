// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.DatabaseMaintenanceComponent15
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  public class DatabaseMaintenanceComponent15 : DatabaseMaintenanceComponent14
  {
    public override IList<ColumnStoreOverlapStatistics> GetClusteredColumnStoreOverlapsMetric(
      int? partitionNumber,
      string tableName,
      string columnName,
      int timeoutInSeconds,
      int minPartitionRowCount)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetOverlapPerSegmentsInPartition", timeoutInSeconds);
      this.BindNullableInt("@partitionNumber", partitionNumber);
      this.BindString("@tableName", tableName, 128, false, SqlDbType.NVarChar);
      this.BindString("@columnName", columnName, 128, false, SqlDbType.NVarChar);
      this.BindInt("@minPartitionRowCount", minPartitionRowCount);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ColumnStoreOverlapStatistics>((ObjectBinder<ColumnStoreOverlapStatistics>) new ColumnStoreOverlapStatisticsBinder());
        return (IList<ColumnStoreOverlapStatistics>) resultCollection.GetCurrent<ColumnStoreOverlapStatistics>().Items;
      }
    }
  }
}
