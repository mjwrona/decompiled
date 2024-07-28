// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CommerceDataHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class CommerceDataHelper
  {
    private const string ALL_EVENTS_FILTER_QUERY = "(EventName eq 'TrialStart') or (EventName eq 'TrialEnd') or (EventName eq 'TrialExtend') or (EventName eq 'NewPurchase') or (EventName eq 'UpgradeQuantity') or (EventName eq 'DowngradeQuantity') or (EventName eq 'CancelPurchase')";
    private const string ACQUISITION_EVENTS_FILTER_QUERY = "(EventName eq 'TrialStart') or (EventName eq 'NewPurchase')";
    private const string SALES_EVENTS_FILTER_QUERY = "(EventName eq 'TrialStart') or (EventName eq 'NewPurchase') or (EventName eq 'UpgradeQuantity') or (EventName eq 'DowngradeQuantity') or (EventName eq 'CancelPurchase')";
    private const string PublisherViewName = "Publisher";

    internal virtual IDictionary<DateTime, CommerceDataProps> GetCommerceStats(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      DateTime afterDate)
    {
      IDictionary<DateTime, CommerceDataProps> commerceStats = (IDictionary<DateTime, CommerceDataProps>) null;
      IVssRequestContext context = requestContext.Elevate();
      IReportingEventsService service = context.GetService<IReportingEventsService>();
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      DateTime dateTime = afterDate;
      DateTime utcNow = DateTime.UtcNow;
      IVssRequestContext requestContext1 = context;
      string resourceName = fullyQualifiedName;
      DateTime startTime = dateTime;
      DateTime endTime = utcNow;
      IEnumerable<ICommerceEvent> commerceEvents = service.GetCommerceEvents(requestContext1, "Publisher", resourceName, startTime, endTime, "(EventName eq 'TrialStart') or (EventName eq 'TrialEnd') or (EventName eq 'TrialExtend') or (EventName eq 'NewPurchase') or (EventName eq 'UpgradeQuantity') or (EventName eq 'DowngradeQuantity') or (EventName eq 'CancelPurchase')");
      if (commerceEvents != null && commerceEvents.Any<ICommerceEvent>())
      {
        requestContext.Trace(12061100, TraceLevel.Info, "gallery", "CommerceDataHelper.GetCommerceStats", string.Format("PublisherViewName:{0}, resource:{1}, startTime:{2}, endTime:{3}, filterQuery:{4}, countOfEvents:{5}", (object) "Publisher", (object) fullyQualifiedName, (object) dateTime, (object) utcNow, (object) "(EventName eq 'TrialStart') or (EventName eq 'TrialEnd') or (EventName eq 'TrialExtend') or (EventName eq 'NewPurchase') or (EventName eq 'UpgradeQuantity') or (EventName eq 'DowngradeQuantity') or (EventName eq 'CancelPurchase')", (object) commerceEvents.Count<ICommerceEvent>()));
        commerceStats = this.AggregateCommerceEvents(requestContext, commerceEvents);
      }
      return commerceStats;
    }

    internal virtual IDictionary<DateTime, CommerceDataProps> AggregateCommerceEvents(
      IVssRequestContext requestContext,
      IEnumerable<ICommerceEvent> commerceEvents)
    {
      IDictionary<DateTime, CommerceDataProps> dictionary = (IDictionary<DateTime, CommerceDataProps>) new Dictionary<DateTime, CommerceDataProps>();
      List<string> computeAccountsCount = new List<string>();
      foreach (ICommerceEvent commerceEvent in commerceEvents)
      {
        DateTime dateTime;
        ref DateTime local = ref dateTime;
        DateTime eventTime = commerceEvent.EventTime;
        int year = eventTime.Year;
        eventTime = commerceEvent.EventTime;
        int month = eventTime.Month;
        eventTime = commerceEvent.EventTime;
        int day = eventTime.Day;
        local = new DateTime(year, month, day);
        if (!dictionary.ContainsKey(dateTime))
          dictionary[dateTime] = new CommerceDataProps();
        CommerceDataProps commerceDataProps = dictionary[dateTime];
        commerceDataProps.EventTime = dateTime;
        string eventName = commerceEvent.EventName;
        if (eventName != null)
        {
          switch (eventName.Length)
          {
            case 8:
              if (eventName == "TrialEnd")
              {
                ++commerceDataProps.TrialEndCount;
                continue;
              }
              break;
            case 10:
              if (eventName == "TrialStart")
              {
                ++commerceDataProps.TrialStartCount;
                continue;
              }
              break;
            case 11:
              switch (eventName[0])
              {
                case 'N':
                  if (eventName == "NewPurchase")
                  {
                    if (this.IsHostedCommerceEvent(requestContext, commerceEvent))
                    {
                      ++commerceDataProps.BuyCountHosted;
                      this.CheckAndIncrementAccountsCount(computeAccountsCount, commerceEvent, dateTime, CommerceReportingEventType.NewPurchase, ref commerceDataProps.NewPurchaseAccountsCountHosted);
                      commerceDataProps.NewPurchaseQuantityHosted += commerceEvent.CurrentQuantity;
                      continue;
                    }
                    ++commerceDataProps.BuyCountConnected;
                    this.CheckAndIncrementAccountsCount(computeAccountsCount, commerceEvent, dateTime, CommerceReportingEventType.NewPurchase, ref commerceDataProps.NewPurchaseAccountsCountConnected);
                    commerceDataProps.NewPurchaseQuantityConnected += commerceEvent.CurrentQuantity;
                    continue;
                  }
                  break;
                case 'T':
                  if (eventName == "TrialExtend")
                  {
                    ++commerceDataProps.TrialExtendCount;
                    continue;
                  }
                  break;
              }
              break;
            case 13:
              if (eventName == "RenewPurchase")
              {
                if (this.IsHostedCommerceEvent(requestContext, commerceEvent))
                {
                  commerceDataProps.RenewalQuantityHosted += commerceEvent.CommittedQuantity;
                  ++commerceDataProps.RenewalAccountsCountHosted;
                  continue;
                }
                commerceDataProps.RenewalQuantityConnected += commerceEvent.CommittedQuantity;
                ++commerceDataProps.RenewalAccountsCountConnected;
                continue;
              }
              break;
            case 14:
              if (eventName == "CancelPurchase")
              {
                if (this.IsHostedCommerceEvent(requestContext, commerceEvent))
                {
                  commerceDataProps.CanceledQuantityHosted += commerceEvent.PreviousQuantity - commerceEvent.CurrentQuantity;
                  this.CheckAndIncrementAccountsCount(computeAccountsCount, commerceEvent, dateTime, CommerceReportingEventType.CancelPurchase, ref commerceDataProps.CanceledAccountsCountHosted);
                  continue;
                }
                commerceDataProps.CanceledQuantityConnected += commerceEvent.PreviousQuantity - commerceEvent.CurrentQuantity;
                this.CheckAndIncrementAccountsCount(computeAccountsCount, commerceEvent, dateTime, CommerceReportingEventType.CancelPurchase, ref commerceDataProps.CanceledAccountsCountConnected);
                continue;
              }
              break;
            case 15:
              if (eventName == "UpgradeQuantity")
              {
                if (this.IsHostedCommerceEvent(requestContext, commerceEvent))
                {
                  commerceDataProps.UpgradeQuantityHosted += commerceEvent.CurrentQuantity - commerceEvent.PreviousQuantity;
                  this.CheckAndIncrementAccountsCount(computeAccountsCount, commerceEvent, dateTime, CommerceReportingEventType.UpgradeQuantity, ref commerceDataProps.UpgradeAccountsCountHosted);
                  continue;
                }
                commerceDataProps.UpgradeQuantityConnected += commerceEvent.CurrentQuantity - commerceEvent.PreviousQuantity;
                this.CheckAndIncrementAccountsCount(computeAccountsCount, commerceEvent, dateTime, CommerceReportingEventType.UpgradeQuantity, ref commerceDataProps.UpgradeAccountsCountConnected);
                continue;
              }
              break;
            case 17:
              if (eventName == "DowngradeQuantity")
              {
                if (this.IsHostedCommerceEvent(requestContext, commerceEvent))
                {
                  commerceDataProps.DowngradeQuantityHosted += commerceEvent.PreviousQuantity - commerceEvent.CurrentQuantity;
                  this.CheckAndIncrementAccountsCount(computeAccountsCount, commerceEvent, dateTime, CommerceReportingEventType.DowngradeQuantity, ref commerceDataProps.DowngradeAccountsCountHosted);
                  continue;
                }
                commerceDataProps.DowngradeQuantityConnected += commerceEvent.PreviousQuantity - commerceEvent.CurrentQuantity;
                this.CheckAndIncrementAccountsCount(computeAccountsCount, commerceEvent, dateTime, CommerceReportingEventType.DowngradeQuantity, ref commerceDataProps.DowngradeAccountsCountConnected);
                continue;
              }
              break;
          }
        }
        throw new NotSupportedException("Event name " + commerceEvent.EventName + " is not supported");
      }
      return dictionary;
    }

    internal virtual bool IsHostedCommerceEvent(
      IVssRequestContext requestContext,
      ICommerceEvent commerceEvent)
    {
      bool flag = true;
      if (commerceEvent.Environment.Equals("Hosted", StringComparison.OrdinalIgnoreCase))
        flag = true;
      else if (commerceEvent.Environment.Equals("OnPremises", StringComparison.OrdinalIgnoreCase))
        flag = false;
      else
        requestContext.TraceAlways(12061100, TraceLevel.Error, "Gallery", "CommerceDataHelper.IsHostedCommerceEvent", "Invalid environment " + commerceEvent.Environment);
      return flag;
    }

    internal virtual List<ExtensionDailyStat> MergeDailyStats(
      List<ExtensionDailyStat> dailyStats,
      IDictionary<DateTime, CommerceDataProps> commerceStats)
    {
      List<CommerceDataProps> list = commerceStats.Values.ToList<CommerceDataProps>();
      List<ExtensionDailyStat> extensionDailyStatList = new List<ExtensionDailyStat>();
      int index1 = 0;
      int index2 = list.Count - 1;
      while (index1 < dailyStats.Count && index2 >= 0)
      {
        if (dailyStats[index1].StatisticDate > list[index2].EventTime)
        {
          extensionDailyStatList.Add(dailyStats[index1]);
          ++index1;
        }
        else if (list[index2].EventTime > dailyStats[index1].StatisticDate)
        {
          CommerceDataProps commerceDataProps = list[index2];
          ExtensionDailyStat stat = new ExtensionDailyStat()
          {
            Version = (string) null,
            StatisticDate = commerceDataProps.EventTime,
            Counts = new EventCounts(),
            ExtendedStats = (IDictionary<string, object>) new Dictionary<string, object>()
          };
          this.AddCommerceDataToDailyStat(stat, commerceDataProps);
          extensionDailyStatList.Add(stat);
          --index2;
        }
        else
        {
          ExtensionDailyStat dailyStat = dailyStats[index1];
          CommerceDataProps commerceDataProps = list[index2];
          dailyStat.ExtendedStats = (IDictionary<string, object>) new Dictionary<string, object>();
          this.AddCommerceDataToDailyStat(dailyStat, commerceDataProps);
          extensionDailyStatList.Add(dailyStat);
          ++index1;
          --index2;
        }
      }
      if (index1 < dailyStats.Count)
      {
        for (; index1 < dailyStats.Count; ++index1)
          extensionDailyStatList.Add(dailyStats[index1]);
      }
      if (index2 >= 0)
      {
        for (; index2 >= 0; --index2)
        {
          CommerceDataProps commerceDataProps = list[index2];
          ExtensionDailyStat stat = new ExtensionDailyStat()
          {
            Version = (string) null,
            StatisticDate = commerceDataProps.EventTime,
            Counts = new EventCounts(),
            ExtendedStats = (IDictionary<string, object>) new Dictionary<string, object>()
          };
          this.AddCommerceDataToDailyStat(stat, commerceDataProps);
          extensionDailyStatList.Add(stat);
        }
      }
      return extensionDailyStatList;
    }

    private void CheckAndIncrementAccountsCount(
      List<string> computeAccountsCount,
      ICommerceEvent commerceEvent,
      DateTime dateTimeKey,
      CommerceReportingEventType eventType,
      ref int countToIncrement)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}-{2}-{3}-{4}", (object) dateTimeKey.Year, (object) dateTimeKey.Month, (object) dateTimeKey.Day, (object) commerceEvent.CollectionId, (object) eventType.ToString());
      if (computeAccountsCount == null || computeAccountsCount.Contains(str))
        return;
      ++countToIncrement;
      computeAccountsCount.Add(str);
    }

    private void AddCommerceDataToDailyStat(
      ExtensionDailyStat stat,
      CommerceDataProps commerceDataProps)
    {
      stat.Counts.TryCount = commerceDataProps.TrialStartCount;
      stat.Counts.BuyCount = commerceDataProps.BuyCountHosted;
      stat.Counts.ConnectedBuyCount = commerceDataProps.BuyCountConnected;
      stat.ExtendedStats.Add("trialEndCount", (object) commerceDataProps.TrialEndCount);
      stat.ExtendedStats.Add("trialExtendCount", (object) commerceDataProps.TrialExtendCount);
      stat.ExtendedStats.Add("newPurchaseQuantityHosted", (object) commerceDataProps.NewPurchaseQuantityHosted);
      stat.ExtendedStats.Add("newPurchaseQuantityConnected", (object) commerceDataProps.NewPurchaseQuantityConnected);
      stat.ExtendedStats.Add("upgradeQuantityHosted", (object) commerceDataProps.UpgradeQuantityHosted);
      stat.ExtendedStats.Add("upgradeQuantityConnected", (object) commerceDataProps.UpgradeQuantityConnected);
      stat.ExtendedStats.Add("downgradeQuantityHosted", (object) commerceDataProps.DowngradeQuantityHosted);
      stat.ExtendedStats.Add("downgradeQuantityConnected", (object) commerceDataProps.DowngradeQuantityConnected);
      stat.ExtendedStats.Add("renewalQuantityHosted", (object) commerceDataProps.RenewalQuantityHosted);
      stat.ExtendedStats.Add("renewalQuantityConnected", (object) commerceDataProps.RenewalQuantityConnected);
      stat.ExtendedStats.Add("canceledQuantityHosted", (object) commerceDataProps.CanceledQuantityHosted);
      stat.ExtendedStats.Add("canceledQuantityConnected", (object) commerceDataProps.CanceledQuantityConnected);
      stat.ExtendedStats.Add("canceledAccountsCountHosted", (object) commerceDataProps.CanceledAccountsCountHosted);
      stat.ExtendedStats.Add("canceledAccountsCountConnected", (object) commerceDataProps.CanceledAccountsCountConnected);
      stat.ExtendedStats.Add("newPurchaseAccountsCountHosted", (object) commerceDataProps.NewPurchaseAccountsCountHosted);
      stat.ExtendedStats.Add("newPurchaseAccountsCountConnected", (object) commerceDataProps.NewPurchaseAccountsCountConnected);
      stat.ExtendedStats.Add("renewalAccountsCountHosted", (object) commerceDataProps.RenewalAccountsCountHosted);
      stat.ExtendedStats.Add("renewalAccountsCountConnected", (object) commerceDataProps.RenewalAccountsCountConnected);
      stat.ExtendedStats.Add("upgradeAccountsCountHosted", (object) commerceDataProps.UpgradeAccountsCountHosted);
      stat.ExtendedStats.Add("upgradeAccountsCountConnected", (object) commerceDataProps.UpgradeAccountsCountConnected);
      stat.ExtendedStats.Add("downgradeAccountsCountHosted", (object) commerceDataProps.DowngradeAccountsCountHosted);
      stat.ExtendedStats.Add("downgradeAccountsCountConnected", (object) commerceDataProps.DowngradeAccountsCountConnected);
    }

    internal virtual List<ExtensionEvent> GetExtensionCommerceEvents(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string eventTypes,
      DateTime? afterDate)
    {
      IVssRequestContext context = requestContext.Elevate();
      IReportingEventsService service = context.GetService<IReportingEventsService>();
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
      DateTime dateTime = DateTime.UtcNow.AddDays(-30.0);
      if (afterDate.HasValue)
        dateTime = afterDate.Value;
      DateTime utcNow = DateTime.UtcNow;
      string str = string.Empty;
      if (eventTypes.Equals("acquisition", StringComparison.OrdinalIgnoreCase))
        str = "(EventName eq 'TrialStart') or (EventName eq 'NewPurchase')";
      else if (eventTypes.Equals("sales", StringComparison.OrdinalIgnoreCase))
        str = "(EventName eq 'TrialStart') or (EventName eq 'NewPurchase') or (EventName eq 'UpgradeQuantity') or (EventName eq 'DowngradeQuantity') or (EventName eq 'CancelPurchase')";
      IVssRequestContext requestContext1 = context;
      string resourceName = fullyQualifiedName;
      DateTime startTime = dateTime;
      DateTime endTime = utcNow;
      string filter = str;
      IEnumerable<ICommerceEvent> commerceEvents = service.GetCommerceEvents(requestContext1, "Publisher", resourceName, startTime, endTime, filter);
      List<ExtensionEvent> extensionCommerceEvents = new List<ExtensionEvent>();
      if (commerceEvents != null && commerceEvents.Any<ICommerceEvent>())
      {
        List<ICommerceEvent> list = commerceEvents.ToList<ICommerceEvent>();
        for (int index = list.Count - 1; index >= 0; --index)
        {
          ICommerceEvent commerceEvent = list[index];
          ExtensionEvent extensionEvent = new ExtensionEvent()
          {
            Id = commerceEvent.EventTime.Ticks,
            StatisticDate = commerceEvent.EventTime,
            Version = (string) null,
            Properties = new JObject()
          };
          extensionEvent.Properties.Add("eventId", (JToken) commerceEvent.EventId);
          extensionEvent.Properties.Add("eventName", (JToken) commerceEvent.EventName);
          extensionEvent.Properties.Add("organizationId", (JToken) commerceEvent.OrganizationId);
          extensionEvent.Properties.Add("collectionId", (JToken) commerceEvent.CollectionId);
          extensionEvent.Properties.Add("collectionName", (JToken) commerceEvent.CollectionName);
          extensionEvent.Properties.Add("subscriptionId", (JToken) commerceEvent.SubscriptionId);
          extensionEvent.Properties.Add("meterName", (JToken) commerceEvent.MeterName);
          extensionEvent.Properties.Add("galleryId", (JToken) commerceEvent.GalleryId);
          extensionEvent.Properties.Add("committedQuantity", (JToken) commerceEvent.CommittedQuantity);
          extensionEvent.Properties.Add("currentQuantity", (JToken) commerceEvent.CurrentQuantity);
          extensionEvent.Properties.Add("previousQuantity", (JToken) commerceEvent.PreviousQuantity);
          extensionEvent.Properties.Add("includedQuantity", (JToken) commerceEvent.IncludedQuantity);
          extensionEvent.Properties.Add("maxQuantity", (JToken) commerceEvent.MaxQuantity);
          extensionEvent.Properties.Add("renewalGroup", (JToken) commerceEvent.RenewalGroup);
          extensionEvent.Properties.Add("eventSource", (JToken) commerceEvent.EventSource);
          extensionEvent.Properties.Add("environment", (JToken) commerceEvent.Environment);
          extensionEvent.Properties.Add("userIdentity", (JToken) commerceEvent.UserIdentity);
          extensionEvent.Properties.Add("trialStartDate", (JToken) commerceEvent.TrialStartDate);
          extensionEvent.Properties.Add("trialEndDate", (JToken) commerceEvent.TrialEndDate);
          extensionEvent.Properties.Add("effectiveDate", (JToken) commerceEvent.EffectiveDate);
          extensionCommerceEvents.Add(extensionEvent);
        }
      }
      return extensionCommerceEvents;
    }

    internal virtual ExtensionEvent GetExtensionCommerceEventById(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string eventId,
      DateTime? eventDateTime,
      string filterString)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
      DateTime dateTime = eventDateTime ?? DateTime.UtcNow;
      DateTime startTime = dateTime.AddMinutes(-1.0);
      DateTime endTime = dateTime.AddMinutes(1.0);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IEnumerable<ICommerceEvent> commerceEvents = vssRequestContext.GetService<IReportingEventsService>().GetCommerceEvents(vssRequestContext, "Publisher", fullyQualifiedName, startTime, endTime, filterString);
      ExtensionEvent commerceEventById = new ExtensionEvent();
      commerceEventById.Properties = new JObject();
      if (commerceEvents != null && commerceEvents.Any<ICommerceEvent>())
      {
        List<ICommerceEvent> list = commerceEvents.ToList<ICommerceEvent>();
        if (list.Count > 0)
          commerceEventById.Properties.Add("vsid", (JToken) list[0].UserIdentity);
      }
      return commerceEventById;
    }
  }
}
