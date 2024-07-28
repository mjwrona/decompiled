// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IExtensionDailyStatsService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (ExtensionDailyStatsService))]
  public interface IExtensionDailyStatsService : IVssFrameworkService
  {
    ExtensionDailyStats GetExtensionDailyStats(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      int? lastNDays,
      ExtensionStatsAggregateType? aggregate,
      DateTime? afterDate);

    void IncrementStatCount(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      DailyStatType statType,
      DateTime? statDate = null,
      bool isBatchingEnabled = false,
      string version = null,
      string targetPlatform = null);

    void IncrementStatCount(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      DailyStatType statType,
      DateTime? statDate = null,
      string targetPlatform = null);

    void RefreshAverageRatingStat(IVssRequestContext requestContext, PublishedExtension extension);

    void AddExtensionEvents(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionEvents> extensionEvents);

    ExtensionEvents GetExtensionEvents(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      int? count,
      DateTime? afterDate,
      string include = null,
      string includeProperty = null);

    ExtensionEvent GetExtensionEventByEventId(
      IVssRequestContext requestContext,
      long eventId,
      Guid extensionId,
      ExtensionLifecycleEventType eventType);

    IEnumerable<ExtensionEvent> GetExtensionEventsByUserId(
      IVssRequestContext requestContext,
      Guid userId);

    List<ExtensionDailyStat> GetExtensionDailyStatsAboveThresholdInstallCount(
      IVssRequestContext requestContext,
      Guid extensionId,
      DateTime afterDate,
      DateTime beforeDate,
      long minInstallCount);

    long GetMaxInstallCountForExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      DateTime afterDate,
      DateTime beforeDate);

    long GetAverageInstallCountsForExtensionVersion(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      DateTime afterDate,
      DateTime beforeDate);

    void UpdateInstallCountStatAboveThresholdInstallCount(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      long installCountToBeUpdated,
      DateTime afterDate,
      DateTime beforeDate,
      long minInstallCount);

    void DecreaseAggregateInstallCountStatistic(
      IVssRequestContext requestContext,
      Guid extensionId,
      long installCountToBeDecreased);

    int AnonymizeExtensionEvents(
      IVssRequestContext requestContext,
      Guid userId,
      IEnumerable<ExtensionEvent> extensionEventsByUser = null);
  }
}
