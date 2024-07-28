// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.DatabaseMaintenanceComponent
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  public class DatabaseMaintenanceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<DatabaseMaintenanceComponent>(12),
      (IComponentCreator) new ComponentCreator<DatabaseMaintenanceComponent13>(13),
      (IComponentCreator) new ComponentCreator<DatabaseMaintenanceComponent14>(14),
      (IComponentCreator) new ComponentCreator<DatabaseMaintenanceComponent15>(15)
    }, "DatabaseMaintenanceService");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static DatabaseMaintenanceComponent()
    {
      DatabaseMaintenanceComponent.s_sqlExceptionFactories.Add(1670018, new SqlExceptionFactory(typeof (NumbersOfRecordsDontMatchForTableException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) new NumbersOfRecordsDontMatchForTableException(sqlError.ExtractString("mainTableName"), sqlError.ExtractLong("oldCount"), sqlError.ExtractLong("tempCount")))));
      DatabaseMaintenanceComponent.s_sqlExceptionFactories.Add(1670012, new SqlExceptionFactory(typeof (FailedToSplitPartitionsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) new FailedToSplitPartitionsException(sqlError.ExtractString("partitionScheme"), sqlError.ExtractInt("newBoundry"), sqlError.ExtractString("errorMessage")))));
    }

    public DatabaseMaintenanceComponent() => this.ContainerErrorCode = 50000;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) DatabaseMaintenanceComponent.s_sqlExceptionFactories;

    public virtual IList<Partition> GetPartitionsWithColumnStoreIndexes()
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetPartitionsWithColumnStoreIndexes", 0);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Partition>((ObjectBinder<Partition>) new PartitionBinder());
        return (IList<Partition>) resultCollection.GetCurrent<Partition>().Items;
      }
    }

    public virtual IList<ColumnStoreOverlapStatistics> GetClusteredColumnStoreOverlapsMetric(
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
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ColumnStoreOverlapStatistics>((ObjectBinder<ColumnStoreOverlapStatistics>) new ColumnStoreOverlapStatisticsBinder());
        return (IList<ColumnStoreOverlapStatistics>) resultCollection.GetCurrent<ColumnStoreOverlapStatistics>().Items;
      }
    }

    public virtual IList<PartitionToSplit> GetPartitionsToSplitMetric()
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetPartitionsToSplitMetric", 0);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PartitionToSplit>((ObjectBinder<PartitionToSplit>) new PartitionToSplitBinder());
        return (IList<PartitionToSplit>) resultCollection.GetCurrent<PartitionToSplit>().Items;
      }
    }

    public virtual int GetPartitionIdFromPartitionNumber(
      int? partitionNumber,
      string partitionScheme)
    {
      int fromPartitionNumber = 0;
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetPartitionId");
      this.BindNullableInt("@partitionNumber", partitionNumber);
      this.BindString("@partitionScheme", partitionScheme, 128, false, SqlDbType.NVarChar);
      object obj = this.ExecuteScalar();
      if (obj != null)
        fromPartitionNumber = Convert.ToInt32(obj);
      return fromPartitionNumber;
    }

    public virtual IList<ColumnStoreFragmentationStatisticRow> GetColumnStoreIndexStats(
      int? partitionNumber,
      string columnStoreIndexName,
      string action,
      int timeoutInSeconds)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_SelectColumnStoreIndexStats", timeoutInSeconds);
      this.BindNullableInt("@physicalPartitionId", partitionNumber);
      this.BindString("@columnStoreIndexName", columnStoreIndexName, 128, false, SqlDbType.NVarChar);
      this.BindString("@action", action, 128, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ColumnStoreFragmentationStatisticRow>((ObjectBinder<ColumnStoreFragmentationStatisticRow>) new ColumnStoreFragmentationStatisticRowBinder());
        return (IList<ColumnStoreFragmentationStatisticRow>) resultCollection.GetCurrent<ColumnStoreFragmentationStatisticRow>().Items;
      }
    }

    public virtual ColumnStoreMaintenanceResult ExecuteColumnStoreMaintenanceOperation(
      string operation,
      int partitionId,
      string schemeName,
      string tableName = null)
    {
      ColumnStoreMaintenanceResult result = new ColumnStoreMaintenanceResult();
      SqlInfoMessageEventHandler messageEventHandler = (SqlInfoMessageEventHandler) ((sender, e) =>
      {
        foreach (SqlError error in e.Errors)
        {
          if (!error.IsStatistical())
            result.Messages.Add(error.Message);
        }
      });
      try
      {
        this.InfoMessage += messageEventHandler;
        this.PrepareStoredProcedure("AnalyticsInternal.prc_MaintainColumnStores", 0);
        this.BindString("@partitionScheme", schemeName, (int) byte.MaxValue, true, SqlDbType.NVarChar);
        this.BindString("@operation", operation, 10, false, SqlDbType.NVarChar);
        this.BindInt("@partitionId", partitionId);
        this.BindString("@tableName", tableName, (int) byte.MaxValue, true, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
        result.Success = true;
      }
      finally
      {
        this.InfoMessage -= messageEventHandler;
      }
      return result;
    }

    public virtual ColumnStoreReorganizeResult ReorganizePartition(
      string tableName,
      string columnStoreIndexName,
      int? partitionNumber,
      int timeoutInSeconds,
      bool compress = false)
    {
      ColumnStoreReorganizeResult result = new ColumnStoreReorganizeResult();
      SqlInfoMessageEventHandler messageEventHandler = (SqlInfoMessageEventHandler) ((sender, e) =>
      {
        foreach (SqlError error in e.Errors)
        {
          if (!error.IsStatistical())
            result.Messages.Add(error.Message);
        }
      });
      try
      {
        this.InfoMessage += messageEventHandler;
        this.PrepareStoredProcedure("AnalyticsInternal.prc_ReorganizePartition", timeoutInSeconds);
        this.BindString("@tableName", tableName, 128, false, SqlDbType.NVarChar);
        this.BindString("@columnStoreIndexName", columnStoreIndexName, 128, false, SqlDbType.NVarChar);
        this.BindNullableInt("@partitionNumber", partitionNumber);
        this.BindBoolean("@compress", compress);
        this.ExecuteNonQuery();
        result.Success = true;
      }
      finally
      {
        this.InfoMessage -= messageEventHandler;
      }
      return result;
    }

    public virtual void SplitNextPartition(int partitionId)
    {
    }

    public virtual void SplitNextPartition()
    {
    }

    public virtual void ClearFailedDatabaseMaintenanceOperations(
      int partitionId,
      int operationTimeThresholdInHours,
      string tableName)
    {
    }
  }
}
