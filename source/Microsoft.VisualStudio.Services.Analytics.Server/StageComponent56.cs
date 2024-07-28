// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageComponent56
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
  internal class StageComponent56 : StageComponent47
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
      this.PrepareStoredProcedure(this.GetMergeStoredProcedure(table, stageVersion));
      this.BindInt("@providerShardId", providerShardId);
      this.BindInt("@streamId", streamId);
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("@operation", replace ? nameof (replace) : "merge", 10, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean("@replace", replace);
      this.BindBoolean("@keysOnly", keysOnly);
      this.BindTable("@records", this.GetTableTypeName(table, stageVersion), records);
      this.BindTable("@extendedFields", this.GetExtendedTypeName(), extendedFields);
      this.BindBoolean("@tableLoading", tableLoading);
      this.BindString("@watermark", watermark, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindNullableInt("@contentVersion", contentVersion);
      this.BindNullableDateTime2("@providerSyncDate", providerSyncDate);
      try
      {
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        if (this.GetSqlErrorNumbers(ex).Any<int>((System.Func<int, bool>) (x => x == 1670025)))
          throw new StageKeysOnlyNotSupportedException(table, providerShardId, streamId);
        throw;
      }
    }
  }
}
