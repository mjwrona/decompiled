// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.DatabaseMaintenanceComponent14
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  public class DatabaseMaintenanceComponent14 : DatabaseMaintenanceComponent13
  {
    public override void ClearFailedDatabaseMaintenanceOperations(
      int partitionId,
      int operationTimeThresholdInHours,
      string tableName)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_ClearFailedDatabaseMaintainenanceOperation");
      this.BindInt("@partitionId", partitionId);
      this.BindInt("@operationTimeThresholdInHours", operationTimeThresholdInHours);
      this.BindString("@tableShortName", tableName, 128, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override void SplitNextPartition()
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_SplitPartitionNextPartition", true);
      this.ExecuteNonQuery();
    }
  }
}
