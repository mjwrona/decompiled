// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent11
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatsComponent11 : ExtensionDailyStatsComponent10
  {
    public virtual void IncrementUninstallCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate,
      string targetPlatform = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementUninstallCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementWebPageViewStat(
      Guid extensionId,
      string version,
      DateTime statisticDate,
      string targetPlatform = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementWebPageViewStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementInstallCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate,
      string targetPlatform = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementInstallCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementDownloadCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate,
      string targetPlatform = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementWebDownloadCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindDate(nameof (statisticDate), statisticDate.ToUniversalTime());
      this.ExecuteNonQuery();
    }

    public virtual void RefreshAverageRatingStat(
      Guid extensionId,
      string fullyQualifiedExtensionName,
      string version,
      DateTime statisticDate,
      string targetPlatform = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_RefreshDailyAverageRatingCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString("productId", fullyQualifiedExtensionName, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public override void ExecuteDailyStatsBatch(
      List<ExtensionDailyStatsUpdateData> extensionDailyStatsCommandData)
    {
      this.PrepareStoredProcedure("Gallery.prc_ExecuteDailyStatsBatch");
      this.BindDailyStatsUpdateDataTable2("dailyStatsCommandBatch", (IEnumerable<ExtensionDailyStatsUpdateData>) extensionDailyStatsCommandData);
      this.ExecuteNonQuery();
    }
  }
}
