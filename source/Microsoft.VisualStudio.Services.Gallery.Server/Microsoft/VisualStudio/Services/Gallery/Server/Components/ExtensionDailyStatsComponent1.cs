// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent1
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatsComponent1 : ExtensionDailyStatsComponent
  {
    public virtual List<ExtensionDailyStat> GetExtensionDailyStats(
      Guid extensionId,
      DateTime afterDate)
    {
      string str = "Gallery.prc_GetExtensionDailyStats";
      this.PrepareStoredProcedure(str);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindDate(nameof (afterDate), afterDate);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionDailyStat>((ObjectBinder<ExtensionDailyStat>) new ExtensionDailyStatBinder());
        return resultCollection.GetCurrent<ExtensionDailyStat>().Items;
      }
    }

    public virtual long GetMaxInstallCountForExtension(
      Guid extensionId,
      DateTime afterDate,
      DateTime beforeDate)
    {
      string str = "Gallery.prc_GetMaxInstallCountForExtension";
      this.PrepareStoredProcedure(str);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindDate(nameof (afterDate), afterDate.ToUniversalTime());
      this.BindDate(nameof (beforeDate), beforeDate.ToUniversalTime());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<long>((ObjectBinder<long>) new ExtensionInstallCountBinder());
        return resultCollection.GetCurrent<long>().Items[0];
      }
    }

    public virtual long GetAverageInstallCountsForExtensionVersion(
      Guid extensionId,
      string version,
      DateTime afterDate,
      DateTime beforeDate)
    {
      string str = "Gallery.prc_GetAverageInstallCountsForExtensionVersion";
      this.PrepareStoredProcedure(str);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindDate(nameof (afterDate), afterDate.ToUniversalTime());
      this.BindDate(nameof (beforeDate), beforeDate.ToUniversalTime());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<long>((ObjectBinder<long>) new ExtensionInstallCountBinder());
        return resultCollection.GetCurrent<long>().Items[0];
      }
    }

    public virtual List<ExtensionDailyStat> GetExtensionDailyStatsAboveThresholdInstallCount(
      Guid extensionId,
      DateTime afterDate,
      DateTime beforeDate,
      long minInstallCount)
    {
      string str = "Gallery.prc_GetExtensionDailyStatsAboveThresholdInstallCount";
      this.PrepareStoredProcedure(str);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindLong("minInstall", minInstallCount);
      this.BindDate(nameof (afterDate), afterDate.ToUniversalTime());
      this.BindDate(nameof (beforeDate), beforeDate.ToUniversalTime());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionDailyStat>((ObjectBinder<ExtensionDailyStat>) new ExtensionDailyStatBinder3());
        return resultCollection.GetCurrent<ExtensionDailyStat>().Items;
      }
    }

    public virtual void UpdateInstallCountStatAboveThresholdInstallCount(
      Guid extensionId,
      string version,
      long installCountToBeUpdated,
      DateTime afterDate,
      DateTime beforeDate,
      long minInstallCount)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateInstallCountStatAboveThresholdInstallCount");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindLong(nameof (installCountToBeUpdated), installCountToBeUpdated);
      this.BindDate(nameof (afterDate), afterDate.ToUniversalTime());
      this.BindDate(nameof (beforeDate), beforeDate.ToUniversalTime());
      this.BindLong("minInstall", minInstallCount);
      this.ExecuteNonQuery();
    }

    public virtual void DecreaseAggregateInstallCountStatistic(
      Guid extensionId,
      long installCountToBeDecreased)
    {
      this.PrepareStoredProcedure("Gallery.prc_DecreaseAggregateInstallCountStatistic");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindLong(nameof (installCountToBeDecreased), installCountToBeDecreased);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementBuyCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementBuyCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementConnectedBuyCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementConnectedBuyCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementConnectedInstallCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementConnectedInstallCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementDownloadCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementDownloadCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementInstallCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementInstallCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementTryCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementTryCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementUninstallCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementUninstallCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementWebPageViewStat(
      Guid extensionId,
      string version,
      DateTime statisticDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementWebPageViewStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDate(nameof (statisticDate), statisticDate);
      this.ExecuteNonQuery();
    }

    public virtual void ExecuteDailyStatsBatch(
      List<ExtensionDailyStatsUpdateData> extensionDailyStatsCommandData)
    {
      throw new NotImplementedException();
    }
  }
}
