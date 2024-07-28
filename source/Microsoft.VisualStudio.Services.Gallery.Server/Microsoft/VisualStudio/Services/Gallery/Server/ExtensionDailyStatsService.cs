// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionDailyStatsService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExtensionDailyStatsService : IExtensionDailyStatsService, IVssFrameworkService
  {
    private const string s_area = "gallery";
    private const string s_layer = "extensiondailystatsservice";
    private readonly CommerceDataHelper _commerceDataHelper;
    private BatchExecutionHandler<ExtensionDailyStatsUpdateData> _batchExecutionHandler;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      int valueFromPath = service.ReadEntries(systemRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/BatchSize/PublisherStat").GetValueFromPath<int>("/Configuration/Service/Gallery/BatchSize/PublisherStat", 100);
      this.InitBatchHandler(systemRequestContext, valueFromPath);
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/BatchSize/PublisherStat");
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this._batchExecutionHandler.Flush(systemRequestContext);
      this._batchExecutionHandler = (BatchExecutionHandler<ExtensionDailyStatsUpdateData>) null;
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
      CustomerIntelligenceData data = new CustomerIntelligenceData();
      data.Add(CustomerIntelligenceProperty.Action, "ExtensionDailyStatsServiceEnd");
      this.PublishCIEvent(systemRequestContext, data);
    }

    public ExtensionDailyStatsService() => this._commerceDataHelper = new CommerceDataHelper();

    internal ExtensionDailyStatsService(CommerceDataHelper commerceDataHelper) => this._commerceDataHelper = commerceDataHelper;

    internal virtual bool BatchProcessor(
      IVssRequestContext requestContext,
      List<ExtensionDailyStatsUpdateData> dataToProcess)
    {
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        ExtensionDailyStatsComponent7 dailyStatsComponent7 = component as ExtensionDailyStatsComponent7;
        if (component != null)
          dailyStatsComponent7.ExecuteDailyStatsBatch(dataToProcess);
      }
      foreach (ExtensionDailyStatsUpdateData dailyStatsUpdateData in dataToProcess)
      {
        CustomerIntelligenceData incrementStatCount = this.GetCIDataForIncrementStatCount(dailyStatsUpdateData.PublisherName, dailyStatsUpdateData.ExtensionName, new Guid?(dailyStatsUpdateData.ExtensionId), dailyStatsUpdateData.Version, dailyStatsUpdateData.StatDate, dailyStatsUpdateData.StatType.ToString(), dailyStatsUpdateData.InstallationTargets, dailyStatsUpdateData.TargetPlatform);
        this.PublishCIEvent(requestContext, incrementStatCount);
      }
      return true;
    }

    public ExtensionDailyStats GetExtensionDailyStats(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      int? lastNDays,
      ExtensionStatsAggregateType? aggregate,
      DateTime? afterDate)
    {
      if (!this.IsExtensionDailyStatsSupported(requestContext))
        throw new ExtensionDailyStatsNotSupportedException(GalleryResources.ExtensionDailyStatsNotSupported());
      if (!this.IsUserLoggedInContext(requestContext))
        throw new ExtensionDailyStatsAnonymousAccessException(GalleryResources.ExtensionDailyStatsUserMustBeLoggedIn());
      ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      PublishedExtension extension = DailyStatsHelper.GetExtension(requestContext, publisherName, extensionName);
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, "extension");
      this.CheckCallerPermissions(requestContext, extension);
      Guid extensionId = extension.ExtensionId;
      if (lastNDays.HasValue)
      {
        lastNDays = lastNDays.Value > 360 || lastNDays.Value <= 0 ? new int?(360) : lastNDays;
        afterDate = new DateTime?(GalleryServerUtil.GetAfterDateForLastNDays(lastNDays));
      }
      DateTime afterDate1 = afterDate ?? DateTime.MinValue;
      if (!afterDate.HasValue || (DateTime.UtcNow - afterDate1).TotalDays > 360.0)
        afterDate1 = DateTime.UtcNow.AddDays(-360.0);
      afterDate1 = new DateTime(afterDate1.Year, afterDate1.Month, afterDate1.Day);
      requestContext.Trace(12061090, TraceLevel.Info, "gallery", nameof (GetExtensionDailyStats), "Getting daily stats for publisherName:{0}, extensionName:{1}, updatedAfterDate:{2}", (object) publisherName, (object) extensionName, (object) afterDate1.ToString((IFormatProvider) CultureInfo.CurrentCulture));
      List<ExtensionDailyStat> extensionDailyStatList = (List<ExtensionDailyStat>) null;
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (component is ExtensionDailyStatsComponent1)
          extensionDailyStatList = (component as ExtensionDailyStatsComponent1).GetExtensionDailyStats(extensionId, afterDate1);
      }
      ExtensionDailyStats extensionDailyStats1 = this.ExtrapolateRnRData(requestContext, extension, new ExtensionDailyStats()
      {
        DailyStats = extensionDailyStatList,
        PublisherName = publisherName,
        ExtensionName = extensionName,
        ExtensionId = extensionId
      }, afterDate1);
      if (aggregate.HasValue)
      {
        ExtensionStatsAggregateType? nullable = aggregate;
        ExtensionStatsAggregateType statsAggregateType = ExtensionStatsAggregateType.Daily;
        if (nullable.GetValueOrDefault() == statsAggregateType & nullable.HasValue)
          extensionDailyStats1.DailyStats = this.GetConsolidatedDailyStatsByStatisticDate(extensionDailyStats1.DailyStats);
      }
      if (this.IsCommerceDataFeatureFlagEnabled(requestContext) && extension != null && this.ShouldReturnPaidExperience(requestContext, extension))
      {
        IDictionary<DateTime, CommerceDataProps> commerceStats = this._commerceDataHelper.GetCommerceStats(requestContext, publisherName, extensionName, afterDate1);
        if (commerceStats != null && commerceStats.Keys.Any<DateTime>())
          extensionDailyStats1.DailyStats = this._commerceDataHelper.MergeDailyStats(extensionDailyStats1.DailyStats, commerceStats);
      }
      extensionDailyStats1.StatCount = extensionDailyStats1.DailyStats != null ? extensionDailyStats1.DailyStats.Count : 0;
      CustomerIntelligenceData extensionDailyStats2 = this.GetCIDataForGetExtensionDailyStats(publisherName, extensionName, lastNDays, afterDate, extensionDailyStats1.StatCount);
      this.PublishCIEvent(requestContext, extensionDailyStats2);
      return extensionDailyStats1;
    }

    internal virtual bool ShouldReturnPaidExperience(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      bool flag = false;
      if (extension.IsFirstPartyAndPaid())
      {
        if (!extension.IsPreview())
          flag = true;
      }
      else if (extension.IsThirdPartyAndPaid())
      {
        string fullyQualifiedName = extension.GetFullyQualifiedName();
        IOfferMeter offerMeter = requestContext.GetService<IOfferMeterService>().GetOfferMeter(requestContext, fullyQualifiedName);
        if (offerMeter != null && offerMeter.FixedQuantityPlans != null && offerMeter.FixedQuantityPlans.Any<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (plan => plan.IsPublic)))
          flag = true;
      }
      return flag;
    }

    public ExtensionEvent GetExtensionEventByEventId(
      IVssRequestContext requestContext,
      long eventId,
      Guid extensionId,
      ExtensionLifecycleEventType eventType)
    {
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      ExtensionEvent extensionEventByEventId = (ExtensionEvent) null;
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (component is ExtensionDailyStatsComponent5)
          extensionEventByEventId = (component as ExtensionDailyStatsComponent5).GetExtensionEventByEventId(eventId, extensionId, eventType);
      }
      return extensionEventByEventId;
    }

    public void IncrementStatCount(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      DailyStatType statType,
      DateTime? statDate = null,
      string targetPlatform = null)
    {
      if (!this.IsExtensionDailyStatsSupported(requestContext))
        return;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(extensionName, nameof (extensionName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(version, nameof (version));
      PublishedExtension extensionVersion = DailyStatsHelper.GetExtensionVersion(requestContext, publisherName, extensionName, version);
      if (!this.IsValidExtensionVersion(extensionVersion))
        throw new ExtensionDailyStatsVersionMismatchException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The specified version {0} is not valid for extension {1}", (object) version, (object) GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName)));
      this.IncrementStatCount(requestContext, extensionVersion, statType, statDate, false, version, targetPlatform);
    }

    public void IncrementStatCount(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      DailyStatType statType,
      DateTime? statDate = null,
      bool isBatchingEnabled = false,
      string version = null,
      string targetPlatform = null)
    {
      if (!this.IsExtensionDailyStatsSupported(requestContext))
        return;
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      Guid extensionId = extension.ExtensionId;
      string publisherName = extension.Publisher?.PublisherName;
      string extensionName = extension.ExtensionName;
      string version1 = GalleryServerUtil.IsValidVersion(version) ? version : this.GetExtensionVersion(extension);
      DateTime dateTime = !statDate.HasValue ? DateTime.UtcNow : statDate.Value;
      string statTypeName = string.Empty;
      List<InstallationTarget> installationTargetList = extension.InstallationTargets != null ? new List<InstallationTarget>((IEnumerable<InstallationTarget>) extension.InstallationTargets) : (List<InstallationTarget>) null;
      if (extension.IsVsCodeExtension())
      {
        if (targetPlatform != null)
          GalleryServerUtil.ValidateIfExtensionVersionEverSupportedTargetPlatform(requestContext, extension, version, targetPlatform);
        else if (!extension.Versions.Any<ExtensionVersion>((Func<ExtensionVersion, bool>) (extensionVersionObj => extensionVersionObj.TargetPlatform == null)))
        {
          targetPlatform = this.GetExtensionLatestVersionTargetPlatform(extension, version1);
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add(CustomerIntelligenceProperty.Action, nameof (IncrementStatCount));
          properties.Add("PublisherName", extension.Publisher?.PublisherName);
          properties.Add("ExtensionName", extension.ExtensionName);
          properties.Add("Version", version);
          properties.Add("TargetPlatform", targetPlatform);
          properties.Add("StatType", statTypeName);
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "UpdateStatisticsForUniversalTargetPlatform", properties);
        }
      }
      else if (extension.IsVsExtension())
      {
        if (!GalleryServerUtil.IsVsixConsolidationEnabledForVsExtension(extension.Metadata))
          targetPlatform = (string) null;
      }
      else
        targetPlatform = (string) null;
      if (isBatchingEnabled)
      {
        this._batchExecutionHandler.Add(requestContext, new ExtensionDailyStatsUpdateData()
        {
          ExtensionName = extensionName,
          PublisherName = publisherName,
          ExtensionId = extensionId,
          Version = version1,
          TargetPlatform = targetPlatform,
          StatType = statType,
          StatDate = dateTime,
          InstallationTargets = installationTargetList
        });
      }
      else
      {
        using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
        {
          if (component is ExtensionDailyStatsComponent11 statsComponent11)
          {
            switch (statType)
            {
              case DailyStatType.Buy:
                statsComponent11.IncrementBuyCountStat(extensionId, version1, dateTime);
                statTypeName = "Buy";
                break;
              case DailyStatType.ConnectedBuy:
                statsComponent11.IncrementConnectedBuyCountStat(extensionId, version1, dateTime);
                statTypeName = "ConnectedBuy";
                break;
              case DailyStatType.ConnectedInstall:
                statsComponent11.IncrementConnectedInstallCountStat(extensionId, version1, dateTime);
                statTypeName = "ConnectedInstall";
                break;
              case DailyStatType.WebDownload:
                statsComponent11.IncrementDownloadCountStat(extensionId, version1, dateTime, targetPlatform);
                statTypeName = "WebDownload";
                break;
              case DailyStatType.Install:
                statsComponent11.IncrementInstallCountStat(extensionId, version1, dateTime, targetPlatform);
                statTypeName = "Install";
                break;
              case DailyStatType.Try:
                statsComponent11.IncrementTryCountStat(extensionId, version1, dateTime);
                statTypeName = "Try";
                break;
              case DailyStatType.Uninstall:
                statsComponent11.IncrementUninstallCountStat(extensionId, version1, dateTime, targetPlatform);
                statTypeName = "Uninstall";
                break;
              case DailyStatType.WebPageView:
                statsComponent11.IncrementWebPageViewStat(extensionId, version1, dateTime, targetPlatform);
                statTypeName = "WebPageView";
                break;
            }
          }
          else
          {
            ExtensionDailyStatsComponent1 dailyStatsComponent1 = component as ExtensionDailyStatsComponent1;
            switch (statType)
            {
              case DailyStatType.Buy:
                dailyStatsComponent1.IncrementBuyCountStat(extensionId, version1, dateTime);
                statTypeName = "Buy";
                break;
              case DailyStatType.ConnectedBuy:
                dailyStatsComponent1.IncrementConnectedBuyCountStat(extensionId, version1, dateTime);
                statTypeName = "ConnectedBuy";
                break;
              case DailyStatType.ConnectedInstall:
                dailyStatsComponent1.IncrementConnectedInstallCountStat(extensionId, version1, dateTime);
                statTypeName = "ConnectedInstall";
                break;
              case DailyStatType.WebDownload:
                dailyStatsComponent1.IncrementDownloadCountStat(extensionId, version1, dateTime);
                statTypeName = "WebDownload";
                break;
              case DailyStatType.Install:
                dailyStatsComponent1.IncrementInstallCountStat(extensionId, version1, dateTime);
                statTypeName = "Install";
                break;
              case DailyStatType.Try:
                dailyStatsComponent1.IncrementTryCountStat(extensionId, version1, dateTime);
                statTypeName = "Try";
                break;
              case DailyStatType.Uninstall:
                dailyStatsComponent1.IncrementUninstallCountStat(extensionId, version1, dateTime);
                statTypeName = "Uninstall";
                break;
              case DailyStatType.WebPageView:
                dailyStatsComponent1.IncrementWebPageViewStat(extensionId, version1, dateTime);
                statTypeName = "WebPageView";
                break;
            }
          }
        }
        CustomerIntelligenceData incrementStatCount = this.GetCIDataForIncrementStatCount(extension.ExtensionName, extension.Publisher?.PublisherName, new Guid?(extension.ExtensionId), version, dateTime, statTypeName, extension.InstallationTargets, targetPlatform);
        this.PublishCIEvent(requestContext, incrementStatCount);
      }
    }

    public void RefreshAverageRatingStat(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (!this.IsExtensionDailyStatsSupported(requestContext))
        return;
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      Guid extensionId = extension.ExtensionId;
      string extensionVersion = this.GetExtensionVersion(extension);
      string versionTargetPlatform = this.GetExtensionLatestVersionTargetPlatform(extension, extensionVersion);
      DateTime utcNow = DateTime.UtcNow;
      string statTypeName = string.Empty;
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (component is ExtensionDailyStatsComponent11 statsComponent11)
        {
          statsComponent11.RefreshAverageRatingStat(extensionId, extension.GetFullyQualifiedName(), extensionVersion, utcNow, versionTargetPlatform);
          statTypeName = "AverageRating";
        }
        else
        {
          (component as ExtensionDailyStatsComponent3).RefreshAverageRatingStat(extensionId, extension.GetFullyQualifiedName(), extensionVersion, utcNow);
          statTypeName = "AverageRating";
        }
      }
      CustomerIntelligenceData forResetStatCount = this.GetCIDataForResetStatCount(extension, extensionVersion, utcNow, statTypeName);
      this.PublishCIEvent(requestContext, forResetStatCount);
    }

    public void AddExtensionEvents(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionEvents> extensionEvents)
    {
      if (!this.IsExtensionDailyStatsSupported(requestContext))
        return;
      ArgumentUtility.CheckForNull<IEnumerable<ExtensionEvents>>(extensionEvents, nameof (extensionEvents));
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) extensionEvents, nameof (extensionEvents));
      foreach (ExtensionEvents extensionEvent1 in extensionEvents)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(extensionEvent1.PublisherName, "evnt.PublisherName");
        ArgumentUtility.CheckStringForNullOrEmpty(extensionEvent1.ExtensionName, "evnt.ExtensionName");
        ArgumentUtility.CheckForNull<IDictionary<string, IEnumerable<ExtensionEvent>>>(extensionEvent1.Events, "evnt.Events");
        if (!extensionEvent1.Events.Keys.Any<string>())
          throw new ExtensionEventsDoNotExistException(GalleryResources.NoEventsForExtension((object) extensionEvent1.ExtensionName, (object) extensionEvent1.PublisherName, (object) extensionEvent1.ExtensionId));
        if (extensionEvent1.ExtensionId == Guid.Empty)
          extensionEvent1.ExtensionId = this.GetExtensionId(requestContext, extensionEvent1.PublisherName, extensionEvent1.ExtensionName);
        this.PrepareExtensionEvents(requestContext, extensionEvent1);
        this.ValidateExtensionEvents(extensionEvent1);
        if (extensionEvent1.Events.ContainsKey("uninstall"))
        {
          IEnumerable<ExtensionEvent> extensionEvents1 = extensionEvent1.Events["uninstall"];
          if (extensionEvents1 != null)
          {
            foreach (ExtensionEvent extensionEvent2 in extensionEvents1)
              this.IncrementStatCount(requestContext, extensionEvent1.PublisherName, extensionEvent1.ExtensionName, extensionEvent2.Version, DailyStatType.Uninstall, new DateTime?(extensionEvent2.StatisticDate), (string) null);
          }
        }
      }
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (component is ExtensionDailyStatsComponent2)
          (component as ExtensionDailyStatsComponent2).AddExtensionEvents(extensionEvents);
      }
      CustomerIntelligenceData addExtensionEvents = this.GetCIDataForAddExtensionEvents(extensionEvents);
      this.PublishCIEvent(requestContext, addExtensionEvents);
    }

    private void PrepareExtensionEvents(IVssRequestContext requestContext, ExtensionEvents evnt)
    {
      if (!evnt.Events.ContainsKey("install"))
        return;
      foreach (ExtensionEvent extensionEvent in evnt.Events["install"])
      {
        if (string.IsNullOrEmpty(extensionEvent.Version))
        {
          PublishedExtension extension = DailyStatsHelper.GetExtension(requestContext, evnt.PublisherName, evnt.ExtensionName);
          if (extension.Flags.HasFlag((Enum) PublishedExtensionFlags.MultiVersion) && extension.Versions != null && extension.Versions.Count > 0)
            extensionEvent.Version = extension.Versions[0].Version.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        }
        if (string.IsNullOrEmpty(extensionEvent.Version))
          requestContext.TraceAlways(12061097, TraceLevel.Error, "gallery", "extensiondailystatsservice", "Version for extension {0}.{1} and eventType:{2} is null or empty", (object) evnt.PublisherName, (object) evnt.ExtensionName, (object) "install");
      }
    }

    public ExtensionEvents GetExtensionEvents(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      int? count,
      DateTime? afterDate,
      string include = null,
      string includeProperty = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(extensionName, nameof (extensionName));
      PublishedExtension extension = DailyStatsHelper.GetExtension(requestContext, publisherName, extensionName);
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, "extension");
      return this.GetExtensionEventsInternal(requestContext, extension, count, afterDate, include, includeProperty);
    }

    public IEnumerable<ExtensionEvent> GetExtensionEventsByUserId(
      IVssRequestContext requestContext,
      Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      IEnumerable<ExtensionEvent> extensionEventsByUserId = (IEnumerable<ExtensionEvent>) null;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.GetExtensionEventsFromView"))
      {
        using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
        {
          if (component is ExtensionDailyStatsComponent10 statsComponent10)
            extensionEventsByUserId = statsComponent10.GetExtensionEventsByUserId(userId.ToString("D").ToUpperInvariant());
        }
      }
      else
      {
        using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
        {
          if (component is ExtensionDailyStatsComponent8 dailyStatsComponent8)
            extensionEventsByUserId = dailyStatsComponent8.GetExtensionEventsByUserId(userId.ToString("D").ToUpperInvariant());
        }
      }
      return extensionEventsByUserId;
    }

    public long GetMaxInstallCountForExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      DateTime afterDate,
      DateTime beforeDate)
    {
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      long countForExtension = 0;
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (component is ExtensionDailyStatsComponent10)
          countForExtension = (component as ExtensionDailyStatsComponent10).GetMaxInstallCountForExtension(extensionId, afterDate, beforeDate);
      }
      return countForExtension;
    }

    public List<ExtensionDailyStat> GetExtensionDailyStatsAboveThresholdInstallCount(
      IVssRequestContext requestContext,
      Guid extensionId,
      DateTime afterDate,
      DateTime beforeDate,
      long minInstallCount)
    {
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      List<ExtensionDailyStat> thresholdInstallCount = (List<ExtensionDailyStat>) null;
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (component is ExtensionDailyStatsComponent10)
          thresholdInstallCount = (component as ExtensionDailyStatsComponent10).GetExtensionDailyStatsAboveThresholdInstallCount(extensionId, afterDate, beforeDate, minInstallCount);
      }
      return thresholdInstallCount;
    }

    public long GetAverageInstallCountsForExtensionVersion(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      DateTime afterDate,
      DateTime beforeDate)
    {
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      ArgumentUtility.CheckForNull<string>(version, nameof (version));
      long extensionVersion = 0;
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (component is ExtensionDailyStatsComponent10)
          extensionVersion = (component as ExtensionDailyStatsComponent10).GetAverageInstallCountsForExtensionVersion(extensionId, version, afterDate, beforeDate);
      }
      return extensionVersion;
    }

    public void UpdateInstallCountStatAboveThresholdInstallCount(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      long installCountToBeUpdated,
      DateTime afterDate,
      DateTime beforeDate,
      long minInstallCount)
    {
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      ArgumentUtility.CheckForNull<string>(version, nameof (version));
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (!(component is ExtensionDailyStatsComponent10))
          return;
        (component as ExtensionDailyStatsComponent10).UpdateInstallCountStatAboveThresholdInstallCount(extensionId, version, installCountToBeUpdated, afterDate, beforeDate, minInstallCount);
      }
    }

    public void DecreaseAggregateInstallCountStatistic(
      IVssRequestContext requestContext,
      Guid extensionId,
      long installCountToBeDecreased)
    {
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      if (installCountToBeDecreased <= 0L)
        return;
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (!(component is ExtensionDailyStatsComponent10))
          return;
        (component as ExtensionDailyStatsComponent10).DecreaseAggregateInstallCountStatistic(extensionId, installCountToBeDecreased);
      }
    }

    public int AnonymizeExtensionEvents(
      IVssRequestContext requestContext,
      Guid userId,
      IEnumerable<ExtensionEvent> extensionEvents = null)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      IEnumerable<ExtensionEvent> extensionEvents1 = extensionEvents ?? this.GetExtensionEventsByUserId(requestContext, userId);
      int num = 0;
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        switch (component)
        {
          case ExtensionDailyStatsComponent9 dailyStatsComponent9:
            foreach (ExtensionEvent extensionEvent in extensionEvents1)
            {
              if (extensionEvent.Properties["vsid"] != null)
                extensionEvent.Properties["vsid"] = (JToken) Guid.Empty;
              if (extensionEvent.Properties["UserDisplayName"] != null)
                extensionEvent.Properties["UserDisplayName"] = (JToken) GalleryServiceConstants.AnonymizedUserName;
              if (extensionEvent.Properties["UserId"] != null && new Guid(extensionEvent.Properties["UserId"].ToString()) == userId)
                extensionEvent.Properties["UserId"] = (JToken) Guid.Empty;
              if (extensionEvent.Properties["ReplyUserId"] != null && new Guid(extensionEvent.Properties["ReplyUserId"].ToString()) == userId)
                extensionEvent.Properties["ReplyUserId"] = (JToken) Guid.Empty;
            }
            num = dailyStatsComponent9.AnonymizeExtensionEvents(extensionEvents1, userId.ToString("D").ToUpperInvariant());
            break;
          case ExtensionDailyStatsComponent8 dailyStatsComponent8:
            num = dailyStatsComponent8.AnonymizeExtensionEvents(userId.ToString("D").ToUpperInvariant());
            break;
        }
      }
      return num;
    }

    private List<ExtensionDailyStat> GetConsolidatedDailyStatsByStatisticDate(
      List<ExtensionDailyStat> dailyStats)
    {
      Hashtable hashtable = new Hashtable();
      List<ExtensionDailyStat> statsByStatisticDate = new List<ExtensionDailyStat>();
      if (dailyStats != null)
      {
        foreach (ExtensionDailyStat dailyStat in dailyStats)
        {
          if (hashtable[(object) dailyStat.StatisticDate] != null)
            hashtable[(object) dailyStat.StatisticDate] = (object) this.GetConsolidatedEventCounts(dailyStat.Counts, (EventCounts) hashtable[(object) dailyStat.StatisticDate]);
          else
            hashtable[(object) dailyStat.StatisticDate] = (object) dailyStat.Counts;
        }
        foreach (ExtensionDailyStat dailyStat in dailyStats)
        {
          if (hashtable[(object) dailyStat.StatisticDate] != null)
          {
            statsByStatisticDate.Add(new ExtensionDailyStat()
            {
              StatisticDate = dailyStat.StatisticDate,
              Counts = (EventCounts) hashtable[(object) dailyStat.StatisticDate]
            });
            hashtable[(object) dailyStat.StatisticDate] = (object) null;
          }
        }
      }
      return statsByStatisticDate;
    }

    protected internal virtual bool IsValidExtensionVersion(PublishedExtension publishedExtension)
    {
      bool flag = true;
      if (publishedExtension == null || publishedExtension.Versions == null)
        flag = false;
      return flag;
    }

    private EventCounts GetConsolidatedEventCounts(EventCounts ec1, EventCounts ec2) => new EventCounts()
    {
      WebPageViews = ec1.WebPageViews + ec2.WebPageViews,
      InstallCount = ec1.InstallCount + ec2.InstallCount,
      WebDownloadCount = ec1.WebDownloadCount + ec2.WebDownloadCount,
      UninstallCount = ec1.UninstallCount + ec2.UninstallCount,
      BuyCount = ec1.BuyCount + ec2.BuyCount,
      TryCount = ec1.TryCount + ec2.TryCount,
      ConnectedInstallCount = ec1.ConnectedInstallCount + ec2.ConnectedInstallCount,
      ConnectedBuyCount = ec1.ConnectedBuyCount + ec2.ConnectedBuyCount,
      AverageRating = ec2.AverageRating
    };

    private ExtensionEvents GetExtensionEventsInternal(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      int? count,
      DateTime? afterDate,
      string include = null,
      string includeProperty = null)
    {
      if (!this.IsExtensionDailyStatsSupported(requestContext))
        throw new ExtensionDailyStatsNotSupportedException(GalleryResources.ExtensionDailyStatsNotSupported());
      if (!this.IsUserLoggedInContext(requestContext))
        throw new ExtensionDailyStatsAnonymousAccessException(GalleryResources.ExtensionDailyStatsUserMustBeLoggedIn());
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      ArgumentUtility.CheckForEmptyGuid(extension.ExtensionId, "extension.ExtensionId");
      if (count.HasValue)
        ArgumentUtility.CheckForNonnegativeInt(count.Value, "count.Value");
      this.CheckCallerPermissions(requestContext, extension);
      ExtensionEvents extensionEvents = new ExtensionEvents();
      extensionEvents.ExtensionName = extension.ExtensionName;
      extensionEvents.PublisherName = extension.Publisher?.PublisherName;
      extensionEvents.ExtensionId = extension.ExtensionId;
      extensionEvents.Events = (IDictionary<string, IEnumerable<ExtensionEvent>>) new Dictionary<string, IEnumerable<ExtensionEvent>>();
      List<string> stringList;
      if (string.IsNullOrWhiteSpace(include))
      {
        stringList = ExtensionLifecycleWellKnownEvents.KnownEventsDictionary.Keys.ToList<string>();
        if (extension != null && this.ShouldReturnPaidExperience(requestContext, extension))
        {
          stringList.Add("sales");
          stringList.Add("acquisition");
        }
        stringList.Add("other");
      }
      else
      {
        string[] collection;
        if (include == null)
          collection = (string[]) null;
        else
          collection = include.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
        stringList = new List<string>((IEnumerable<string>) collection);
      }
      foreach (string key in stringList)
      {
        List<ExtensionEvent> extensionEventList = (List<ExtensionEvent>) null;
        ExtensionLifecycleEventType eventType;
        if (ExtensionLifecycleWellKnownEvents.KnownEventsDictionary.TryGetValue(key, out eventType))
          extensionEventList = this.GetExtensionEventsById(requestContext, extension.ExtensionId, eventType, count, afterDate);
        else if (key.Equals("other", StringComparison.OrdinalIgnoreCase))
          extensionEventList = this.GetExtensionEventsById(requestContext, extension.ExtensionId, ExtensionLifecycleEventType.Other, count, afterDate);
        else if (key.Equals("acquisition", StringComparison.OrdinalIgnoreCase))
        {
          if (this.IsCommerceDataFeatureFlagEnabled(requestContext) && extension != null && this.ShouldReturnPaidExperience(requestContext, extension))
            extensionEventList = this._commerceDataHelper.GetExtensionCommerceEvents(requestContext, extension, "acquisition", afterDate);
        }
        else if (key.Equals("sales", StringComparison.OrdinalIgnoreCase) && this.IsCommerceDataFeatureFlagEnabled(requestContext) && extension != null && this.ShouldReturnPaidExperience(requestContext, extension))
          extensionEventList = this._commerceDataHelper.GetExtensionCommerceEvents(requestContext, extension, "sales", afterDate);
        extensionEvents.Events.Add(key, (IEnumerable<ExtensionEvent>) extensionEventList);
      }
      if (!string.IsNullOrWhiteSpace(includeProperty) && string.Compare(includeProperty, GalleryServiceConstants.LastContactDetails, StringComparison.OrdinalIgnoreCase) == 0 && extensionEvents.Events != null)
      {
        if (extensionEvents.Events.ContainsKey("uninstall"))
          this.AddLastContactProperty(requestContext, extensionEvents, "uninstall", "hostName", "uninstall");
        if (extensionEvents.Events.ContainsKey("sales"))
          this.AddLastContactProperty(requestContext, extensionEvents, "sales", "collectionName", "salesAndAcquisition");
        if (extensionEvents.Events.ContainsKey("acquisition"))
          this.AddLastContactProperty(requestContext, extensionEvents, "acquisition", "collectionName", "salesAndAcquisition");
      }
      CustomerIntelligenceData getExtensionEvents = this.GetCIDataForGetExtensionEvents(extension, count, afterDate, extensionEvents.Events);
      this.PublishCIEvent(requestContext, getExtensionEvents);
      return extensionEvents;
    }

    private void AddLastContactProperty(
      IVssRequestContext requestContext,
      ExtensionEvents extensionEvents,
      string eventType,
      string hostTokenName,
      string lastContactKeySuffix)
    {
      IDictionary<string, DateTime> lastContactDetails = this.GetLastContactDetails(requestContext, extensionEvents.PublisherName, extensionEvents.ExtensionName);
      IEnumerable<ExtensionEvent> extensionEvents1 = extensionEvents.Events[eventType];
      if (extensionEvents1 == null || lastContactDetails == null || lastContactDetails.Count <= 0)
        return;
      foreach (ExtensionEvent extensionEvent in extensionEvents1)
      {
        string key1 = (string) extensionEvent.Properties.SelectToken(hostTokenName);
        string key2 = key1 + "." + lastContactKeySuffix;
        if (key1 != null && lastContactDetails.ContainsKey(key1))
          extensionEvent.Properties["lastContact"] = (JToken) lastContactDetails[key1];
        if (key2 != null && lastContactDetails.ContainsKey(key2))
          extensionEvent.Properties["lastContact"] = (JToken) lastContactDetails[key2];
      }
    }

    private IDictionary<string, DateTime> GetLastContactDetails(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      IDictionary<string, DateTime> lastContactDetails = (IDictionary<string, DateTime>) new Dictionary<string, DateTime>();
      List<CustomerLastContact> customerLastContactList = new List<CustomerLastContact>();
      try
      {
        ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
        ArtifactSpec artifactSpec1 = new ArtifactSpec(GalleryServiceConstants.CreateExtensionLastContactArtifactKind, GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName), 0);
        IVssRequestContext requestContext1 = requestContext;
        ArtifactSpec artifactSpec2 = artifactSpec1;
        using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpec2, (IEnumerable<string>) null))
        {
          foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
          {
            foreach (PropertyValue propertyValue in current.PropertyValues)
            {
              if (propertyValue.PropertyName != null)
              {
                if (propertyValue.Value != null)
                {
                  try
                  {
                    customerLastContactList.Add(new CustomerLastContact()
                    {
                      Account = propertyValue.PropertyName,
                      LastContactDate = Convert.ToDateTime(propertyValue.Value, (IFormatProvider) CultureInfo.InvariantCulture)
                    });
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(12061097, "gallery", "extensiondailystatsservice", ex);
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061097, "gallery", "extensiondailystatsservice", ex);
      }
      foreach (CustomerLastContact customerLastContact in customerLastContactList)
        lastContactDetails[customerLastContact.Account] = customerLastContact.LastContactDate;
      return lastContactDetails;
    }

    protected internal virtual bool IsUserLoggedInContext(IVssRequestContext requestContext) => requestContext.UserContext != (IdentityDescriptor) null;

    protected internal virtual Guid GetExtensionId(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      PublishedExtension extension = DailyStatsHelper.GetExtension(requestContext, publisherName, extensionName);
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, "extension");
      return extension.ExtensionId;
    }

    protected internal virtual void CheckCallerPermissions(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      ArgumentUtility.CheckForNull<PublisherFacts>(extension.Publisher, "extension.Publisher");
      ArgumentUtility.CheckForNull<string>(extension.Publisher.PublisherName, "extension.Publisher.PublisherName");
      Publisher publisher = this.GetPublisher(requestContext, extension.Publisher.PublisherName);
      if (!GallerySecurity.HasPublisherPermission(requestContext, publisher, PublisherPermissions.Read | PublisherPermissions.PrivateRead | PublisherPermissions.ViewPermissions) && !GallerySecurity.HasExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false))
        throw new ExtensionDailyStatsAccessDeniedException(GalleryResources.ExtensionDailyStatsAccessDenied());
    }

    protected internal virtual string GetExtensionVersion(PublishedExtension extension)
    {
      string stringVar = (string) null;
      if (extension != null && extension.Versions != null && extension.Versions.Count > 0)
        stringVar = extension.Versions[0].Version;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar, "version");
      return stringVar;
    }

    protected internal virtual string GetExtensionLatestVersionTargetPlatform(
      PublishedExtension extension,
      string version)
    {
      if (extension == null || extension.Versions == null || extension.Versions.Count <= 0)
        return (string) null;
      return (extension.Versions.Find((Predicate<ExtensionVersion>) (extensionVersion => extensionVersion.Version.Equals(version))) ?? throw new ExtensionVersionNotFoundException(GalleryWebApiResources.ExtensionVersionNotFound((object) GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName), (object) version.ToString()))).TargetPlatform;
    }

    private List<ExtensionEvent> GetExtensionEventsById(
      IVssRequestContext requestContext,
      Guid extensionId,
      ExtensionLifecycleEventType eventType,
      int? count,
      DateTime? afterDate)
    {
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      List<ExtensionEvent> extensionEventsById = (List<ExtensionEvent>) null;
      using (ExtensionDailyStatsComponent component = requestContext.CreateComponent<ExtensionDailyStatsComponent>())
      {
        if (component is ExtensionDailyStatsComponent2)
          extensionEventsById = (component as ExtensionDailyStatsComponent2).GetExtensionEvents(extensionId, eventType, count, afterDate);
      }
      return extensionEventsById;
    }

    private ExtensionDailyStats ExtrapolateRnRData(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ExtensionDailyStats extensionDailyStats,
      DateTime afterDate)
    {
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ExtensionDailyStats>(extensionDailyStats, nameof (extensionDailyStats));
      if (extensionDailyStats.DailyStats != null)
      {
        if (afterDate < ExtensionDailyStatsConstants.AverageRatingLoggingStartDate)
          afterDate = ExtensionDailyStatsConstants.AverageRatingLoggingStartDate;
        ExtensionDailyStat extensionDailyStat = extensionDailyStats.DailyStats.FindLast((Predicate<ExtensionDailyStat>) (x => x.StatisticDate.Equals(afterDate)));
        if (extensionDailyStat == null || (double) extensionDailyStat.Counts.AverageRating <= 0.0)
        {
          ReviewSummary reviewSummary = requestContext.GetService<IRatingAndReviewService>().GetReviewSummary(requestContext, extension, new DateTime?(afterDate == DateTime.MaxValue ? afterDate : afterDate.AddDays(1.0)));
          if ((double) reviewSummary.AverageRating > 0.0)
          {
            if (extensionDailyStat == null)
            {
              string str = extensionDailyStats.DailyStats.Count > 0 ? extensionDailyStats.DailyStats[extensionDailyStats.DailyStats.Count - 1].Version : extension.Versions?[0].Version;
              extensionDailyStat = new ExtensionDailyStat()
              {
                Counts = new EventCounts(),
                StatisticDate = afterDate,
                Version = str
              };
              extensionDailyStats.DailyStats.Add(extensionDailyStat);
              extensionDailyStats.StatCount = extensionDailyStats.DailyStats.Count;
            }
            extensionDailyStat.Counts.AverageRating = reviewSummary.AverageRating;
          }
        }
        extensionDailyStats.DailyStats.Sort((Comparison<ExtensionDailyStat>) ((x, y) => !(x.StatisticDate == y.StatisticDate) ? (x.StatisticDate > y.StatisticDate ? -1 : 1) : (new Version(x.Version) > new Version(y.Version) ? -1 : 1)));
        for (int index = extensionDailyStats.DailyStats.Count - 2; index >= 0; --index)
        {
          if ((double) extensionDailyStats.DailyStats[index].Counts.AverageRating <= 0.0)
            extensionDailyStats.DailyStats[index].Counts.AverageRating = extensionDailyStats.DailyStats[index + 1].Counts.AverageRating;
        }
      }
      return extensionDailyStats;
    }

    private void ValidateExtensionEvents(ExtensionEvents evnt)
    {
      ArgumentUtility.CheckForEmptyGuid(evnt.ExtensionId, "envt.ExtensionId");
      foreach (string key in (IEnumerable<string>) evnt.Events.Keys)
      {
        IEnumerable<ExtensionEvent> extensionEvents = evnt.Events[key];
        if (extensionEvents != null)
        {
          foreach (ExtensionEvent extensionEvent in extensionEvents)
            ArgumentUtility.CheckStringForNullOrWhiteSpace(extensionEvent.Version, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Version for extension {0}.{1} and eventType:{2} is null or empty", (object) evnt.PublisherName, (object) evnt.ExtensionName, (object) key));
        }
      }
    }

    private Publisher GetPublisher(IVssRequestContext requestContext, string publisherName)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IPublisherService>().QueryPublisher(vssRequestContext, publisherName, PublisherQueryFlags.None);
    }

    protected internal virtual bool IsCommerceDataFeatureFlagEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Publisher360.EnableCommerceData");
    }

    private bool IsExtensionDailyStatsSupported(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment;

    private CustomerIntelligenceData GetCIDataForGetExtensionDailyStats(
      string publisherName,
      string extensionName,
      int? lastNDays,
      DateTime? afterDate,
      int statCount)
    {
      CustomerIntelligenceData extensionDailyStats = new CustomerIntelligenceData();
      extensionDailyStats.Add(CustomerIntelligenceProperty.Action, "GetExtensionDailyStats");
      extensionDailyStats.Add("PublisherName", publisherName);
      extensionDailyStats.Add("ExtensionName", extensionName);
      if (lastNDays.HasValue)
        extensionDailyStats.Add(nameof (lastNDays), (double) lastNDays.Value);
      if (afterDate.HasValue)
        extensionDailyStats.Add("AfterDate", (object) afterDate.Value);
      extensionDailyStats.Add("RecordCountFromDB", (double) statCount);
      return extensionDailyStats;
    }

    private CustomerIntelligenceData GetCIDataForIncrementStatCount(
      string extensionName,
      string publisherName,
      Guid? extensionGuid,
      string version,
      DateTime statDate,
      string statTypeName,
      List<InstallationTarget> installationTargets,
      string targetPlatform)
    {
      CustomerIntelligenceData incrementStatCount = new CustomerIntelligenceData();
      incrementStatCount.Add(CustomerIntelligenceProperty.Action, "IncrementStatCount");
      incrementStatCount.Add("ExtensionName", extensionName);
      if (publisherName != null)
        incrementStatCount.Add("PublisherName", publisherName);
      if (installationTargets != null && installationTargets.Count > 0)
      {
        string str = string.Join(";", installationTargets.Select<InstallationTarget, string>((Func<InstallationTarget, string>) (x => x == null ? string.Empty : x.Target)));
        incrementStatCount.Add("InstallationTargets", str);
        incrementStatCount.Add("ProductType", GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) installationTargets));
      }
      if (extensionGuid.HasValue)
        incrementStatCount.Add("ExtensionId", (object) extensionGuid.Value);
      incrementStatCount.Add("Version", version);
      incrementStatCount.Add("TargetPlatform", targetPlatform);
      incrementStatCount.Add("StatisticDate", (object) statDate);
      incrementStatCount.Add("StatType", statTypeName);
      return incrementStatCount;
    }

    private CustomerIntelligenceData GetCIDataForResetStatCount(
      PublishedExtension extension,
      string version,
      DateTime statDate,
      string statTypeName)
    {
      CustomerIntelligenceData forResetStatCount = new CustomerIntelligenceData();
      forResetStatCount.Add(CustomerIntelligenceProperty.Action, "ResetStatCount");
      forResetStatCount.Add("PublisherName", extension.Publisher?.PublisherName);
      forResetStatCount.Add("ExtensionName", extension.ExtensionName);
      forResetStatCount.Add("Version", version);
      forResetStatCount.Add("StatisticDate", (object) statDate);
      forResetStatCount.Add("StatType", statTypeName);
      return forResetStatCount;
    }

    private CustomerIntelligenceData GetCIDataForAddExtensionEvents(
      IEnumerable<ExtensionEvents> extensionEvents)
    {
      CustomerIntelligenceData addExtensionEvents = new CustomerIntelligenceData();
      addExtensionEvents.Add(CustomerIntelligenceProperty.Action, "AddExtensionEvents");
      int num = extensionEvents.Count<ExtensionEvents>();
      addExtensionEvents.Add("CountOfExtensions", (double) num);
      for (int index = 0; index < num; ++index)
      {
        ExtensionEvents extensionEvents1 = extensionEvents.ElementAt<ExtensionEvents>(index);
        addExtensionEvents.Add("ExtensionId" + index.ToString(), (object) extensionEvents1.ExtensionId);
        addExtensionEvents.Add("PublisherName" + index.ToString(), extensionEvents1.PublisherName);
        addExtensionEvents.Add("ExtensionName" + index.ToString(), extensionEvents1.ExtensionName);
        addExtensionEvents.Add("EventTypes", string.Join(",", (IEnumerable<string>) extensionEvents1.Events.Keys));
      }
      return addExtensionEvents;
    }

    private CustomerIntelligenceData GetCIDataForGetExtensionEvents(
      PublishedExtension extension,
      int? count,
      DateTime? afterDate,
      IDictionary<string, IEnumerable<ExtensionEvent>> extensionEvents)
    {
      CustomerIntelligenceData getExtensionEvents = new CustomerIntelligenceData();
      getExtensionEvents.Add(CustomerIntelligenceProperty.Action, "GetExtensionEvents");
      getExtensionEvents.Add("PublisherName", extension.Publisher?.PublisherName);
      getExtensionEvents.Add("ExtensionName", extension.ExtensionName);
      if (count.HasValue)
        getExtensionEvents.Add("Count", (double) count.Value);
      if (afterDate.HasValue)
        getExtensionEvents.Add("AfterDate", (object) afterDate.Value);
      foreach (string key in (IEnumerable<string>) extensionEvents.Keys)
      {
        CustomerIntelligenceData intelligenceData = getExtensionEvents;
        string name = key;
        IEnumerable<ExtensionEvent> extensionEvent = extensionEvents[key];
        // ISSUE: variable of a boxed type
        __Boxed<int?> local = (ValueType) (extensionEvent != null ? new int?(extensionEvent.Count<ExtensionEvent>()) : new int?());
        intelligenceData.Add(name, (object) local);
      }
      return getExtensionEvents;
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      int valueFromPath = changedEntries.GetValueFromPath<int>("/Configuration/Service/Gallery/BatchSize/PublisherStat", 100);
      this.InitBatchHandler(requestContext, valueFromPath);
    }

    private void InitBatchHandler(IVssRequestContext requestContext, int batchSize)
    {
      if (this._batchExecutionHandler != null)
      {
        this._batchExecutionHandler.Flush(requestContext);
        this._batchExecutionHandler = (BatchExecutionHandler<ExtensionDailyStatsUpdateData>) null;
      }
      this._batchExecutionHandler = new BatchExecutionHandler<ExtensionDailyStatsUpdateData>(batchSize, new Func<IVssRequestContext, List<ExtensionDailyStatsUpdateData>, bool>(this.BatchProcessor));
    }

    private void PublishCIEvent(IVssRequestContext requestContext, CustomerIntelligenceData data)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Publisher360", data);
    }
  }
}
