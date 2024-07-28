// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageComponent23
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StageComponent23 : StageComponent21
  {
    public override void InvalidateTableAllPartitions(string table)
    {
      this.GetLatestStagingSchema(table);
      this.PrepareStoredProcedure("AnalyticsInternal.prc_InvalidateTableProviderShard", false);
      this.BindNullableInt("@partitionId", new int?());
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindNullableInt("@providerShardId", new int?());
      this.BindBoolean("@disableCurrentStream", false);
      this.ExecuteNonQuery();
    }
  }
}
