// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageComponent18
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StageComponent18 : StageComponent12
  {
    public override CleanupStreamResult CleanupStream(
      string table,
      int providerShardId,
      int streamId,
      int commandTimeoutSeconds)
    {
      return this.CleanupStreamGeneric(table, providerShardId, streamId, commandTimeoutSeconds);
    }

    protected CleanupStreamResult CleanupStreamGeneric(
      string table,
      int providerShardId,
      int streamId,
      int commandTimeoutSeconds)
    {
      string parameterValue1 = (string) null;
      string parameterValue2 = (string) null;
      string parameterValue3 = (string) null;
      string parameterValue4 = (string) null;
      string dbTableName = this.GetDbTableName(table);
      string extendedTableName = this.GetDbExtendedTableName(table);
      string tableNamePattern = this.GetDbCustomTableNamePattern(table);
      char[] chArray = new char[1]{ '.' };
      string[] strArray1 = dbTableName.Split(chArray);
      string parameterValue5 = strArray1[0];
      string parameterValue6 = strArray1[1];
      if (extendedTableName != null)
      {
        string[] strArray2 = extendedTableName.Split('.');
        parameterValue1 = strArray2[0];
        parameterValue2 = strArray2[1];
      }
      if (tableNamePattern != null)
      {
        string[] strArray3 = tableNamePattern.Split('.');
        parameterValue3 = strArray3[0];
        parameterValue4 = strArray3[1];
      }
      if (parameterValue6 == null)
        throw new InvalidOperationException(AnalyticsResources.UNKNOWN_TABLE_NAME((object) parameterValue6));
      this.PrepareStoredProcedure("AnalyticsInternal.prc_CleanupStream", commandTimeoutSeconds);
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("@providerShardId", providerShardId);
      this.BindInt("@streamId", streamId);
      this.BindString("@dbTableSchemaName", parameterValue5, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("@dbTableName", parameterValue6, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("@dbExtendedTableSchemaName", parameterValue1, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("@dbExtendedTableName", parameterValue2, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("@dbCustomTableSchemaName", parameterValue3, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("@dbCustomTableNamePattern", parameterValue4, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CleanupStreamResult>((ObjectBinder<CleanupStreamResult>) new CleanupStreamColumns());
        return resultCollection.GetCurrent<CleanupStreamResult>().Items.Single<CleanupStreamResult>();
      }
    }

    public override CleanupStreamResult CleanupBatches(int retainBatchHistoryDays)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_CleanupBatches");
      this.BindInt("@retainBatchHistoryDays", retainBatchHistoryDays);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CleanupStreamResult>((ObjectBinder<CleanupStreamResult>) new CleanupStreamColumns());
        return resultCollection.GetCurrent<CleanupStreamResult>().Items.Single<CleanupStreamResult>();
      }
    }
  }
}
