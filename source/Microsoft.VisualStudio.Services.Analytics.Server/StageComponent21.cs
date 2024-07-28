// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageComponent21
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StageComponent21 : StageComponent19
  {
    protected override void InvalidateProviderShardImpl(
      string table,
      int? providerShardId,
      IEnumerable<string> fieldNames = null,
      bool disableCurrentStream = false,
      bool keysOnly = false)
    {
      if (string.IsNullOrEmpty(table))
      {
        if (providerShardId.HasValue)
          throw new ArgumentException(AnalyticsResources.ARGUMENT_HAS_TO_BE_NULL((object) nameof (providerShardId), (object) nameof (table)), nameof (providerShardId));
      }
      else
      {
        SqlMetaData[] latestStagingSchema = this.GetLatestStagingSchema(table);
        if (fieldNames != null && !((IEnumerable<SqlMetaData>) latestStagingSchema).Select<SqlMetaData, string>((System.Func<SqlMetaData, string>) (meta => meta.Name)).Intersect<string>(fieldNames, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase).Any<string>())
          return;
      }
      this.PrepareStoredProcedure("AnalyticsInternal.prc_InvalidateTableProviderShard");
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindNullableInt("@providerShardId", providerShardId);
      this.BindBoolean("@disableCurrentStream", disableCurrentStream);
      this.ExecuteNonQuery();
    }
  }
}
