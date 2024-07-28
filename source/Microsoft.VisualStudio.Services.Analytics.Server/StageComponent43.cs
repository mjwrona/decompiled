// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageComponent43
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StageComponent43 : StageComponent33
  {
    public override void StageRecords(
      string table,
      int providerShardId,
      int streamId,
      IEnumerable<SqlDataRecord> records,
      IEnumerable<SqlDataRecord> extendedFields,
      bool replace,
      bool keysOnly,
      bool tableLoading,
      string watermark,
      int stageVersion,
      int? contentVersion,
      DateTime? providerSyncDate,
      out bool terminate)
    {
      terminate = false;
      string mergeStoredProcedure = this.GetMergeStoredProcedure(table, stageVersion);
      try
      {
        this.BeginTransaction(IsolationLevel.ReadCommitted);
        this.SetWatermark(table, providerShardId, streamId, watermark, contentVersion);
        this.PrepareStoredProcedure(mergeStoredProcedure);
        this.BindInt("@providerShardId", providerShardId);
        this.BindInt("@streamId", streamId);
        this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindString("@operation", replace ? nameof (replace) : "merge", 10, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindBoolean("@replace", replace);
        this.BindBoolean("@keysOnly", keysOnly);
        this.BindTable("@records", this.GetTableTypeName(table, stageVersion), records);
        this.BindTable("@extendedFields", this.GetExtendedTypeName(), extendedFields);
        this.BindBoolean("@tableLoading", tableLoading);
        this.ExecuteNonQuery();
        this.CommitTransaction();
      }
      catch (SqlException ex)
      {
        this.RollbackTransaction();
        if (this.GetSqlErrorNumbers(ex).Any<int>((System.Func<int, bool>) (x => x == 1670025)))
          throw new StageKeysOnlyNotSupportedException(table, providerShardId, streamId);
        throw;
      }
      catch
      {
        this.RollbackTransaction();
        throw;
      }
    }
  }
}
