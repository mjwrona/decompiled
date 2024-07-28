// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageComponent19
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StageComponent19 : StageComponent18
  {
    public override void UpdateStream(
      string table,
      int providerShardId,
      int streamId,
      bool enabled,
      bool current,
      int? priority,
      bool disposed,
      bool? keysOnly)
    {
      if (keysOnly.GetValueOrDefault())
        throw new NotSupportedException(AnalyticsResources.KEYS_ONLY_NOT_SUPPORTED());
      this.PrepareStoredProcedure("AnalyticsInternal.prc_UpdateStream");
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("@providerShardId", providerShardId);
      this.BindInt("@streamId", streamId);
      this.BindBoolean("@enabled", enabled);
      this.BindBoolean("@current", current);
      this.BindNullableInt("@priority", priority);
      this.BindBoolean("@disposed", disposed);
      this.ExecuteNonQuery();
    }

    public override void SetStagingTableMaintenance(
      string table,
      bool enable,
      string maintenanceReason)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_SetStagingTableMaintenance", false);
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean("@maintenance", enable);
      this.ExecuteNonQuery();
    }
  }
}
