// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public sealed class EventNotificationService : 
    IEventNotificationService,
    IVssFrameworkService,
    IEventNotificationServiceInternal
  {
    private static readonly HashSet<NotificationStatisticType> s_statTypesAllowingEmptyPath = new HashSet<NotificationStatisticType>()
    {
      NotificationStatisticType.HourlyEvents,
      NotificationStatisticType.Events,
      NotificationStatisticType.Notifications,
      NotificationStatisticType.HourlyNotifications
    };
    private int m_suspendNotificationBatchSize = 1000;
    private const string s_area = "Notifications";
    private const string s_layer = "EventNotificationService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_suspendNotificationBatchSize = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) NotificationFrameworkConstants.SuspendNotificationBatchSize, this.m_suspendNotificationBatchSize);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void SuspendUnprocessedNotifications(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      NotificationQueryCondition notificationQueryCondition = new NotificationQueryCondition()
      {
        SubscriptionId = subscription.SubscriptionId
      };
      this.SuspendUnprocessedNotificationsInternal(requestContext, new List<NotificationQueryCondition>()
      {
        notificationQueryCondition
      });
    }

    public void SuspendUnprocessedNotifications(
      IVssRequestContext requestContext,
      List<NotificationQueryCondition> notificationKeys,
      bool requireNamespaceAdmin = true)
    {
      if (requireNamespaceAdmin)
      {
        foreach (NotificationQueryCondition notificationKey in notificationKeys)
        {
          if (notificationKey.EventInitiator == Guid.Empty && notificationKey.Subscriber == Guid.Empty && string.IsNullOrEmpty(notificationKey.SubscriptionId))
            throw new ArgumentException(CoreRes.InvalidNotificationKey());
          if (!NotificationSubscriptionSecurityUtils.CallerHasAdminPermissions(requestContext, 2))
            throw new UnauthorizedAccessException(CoreRes.UnauthorizedSuspend());
        }
      }
      this.SuspendUnprocessedNotificationsInternal(requestContext, notificationKeys);
    }

    internal void SuspendUnprocessedNotificationsInternal(
      IVssRequestContext requestContext,
      List<NotificationQueryCondition> notificationKeys)
    {
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
        component.SuspendUnprocessedNotifications((IEnumerable<NotificationQueryCondition>) notificationKeys, this.m_suspendNotificationBatchSize);
    }

    private IEnumerable<NotificationStatisticEntry> ValidateStats(
      IVssRequestContext requestContext,
      IEnumerable<NotificationStatisticEntry> stats)
    {
      List<NotificationStatisticEntry> notificationStatisticEntryList = new List<NotificationStatisticEntry>();
      if (stats != null)
      {
        foreach (NotificationStatisticEntry stat in stats)
        {
          DateTime date = stat.Date;
          if (stat.HitCount <= 0)
            requestContext.Trace(1002212, TraceLevel.Info, "Notifications", nameof (EventNotificationService), string.Format("HitCount is 0 {0}", (object) stat.Type));
          else if (!Enum.IsDefined(typeof (NotificationStatisticType), (object) stat.Type) || stat.Type == NotificationStatisticType.UnprocessedRangeStart || stat.Type == NotificationStatisticType.HourlyRangeStart || stat.Type == NotificationStatisticType.DelayRangeStart)
            requestContext.Trace(1002212, TraceLevel.Error, "Notifications", nameof (EventNotificationService), string.Format("Type is invalad in {0}", (object) stat.Type));
          else if (stat.Path == null && !EventNotificationService.s_statTypesAllowingEmptyPath.Contains(stat.Type))
            requestContext.Trace(1002212, TraceLevel.Error, "Notifications", nameof (EventNotificationService), string.Format("Type should have path {0}", (object) stat.Type));
          else
            notificationStatisticEntryList.Add(stat);
        }
      }
      else
        requestContext.Trace(1002212, TraceLevel.Warning, "Notifications", nameof (EventNotificationService), "Stats is null");
      return (IEnumerable<NotificationStatisticEntry>) notificationStatisticEntryList;
    }

    public void UpdateNotificationStatistics(
      IVssRequestContext requestContext,
      IEnumerable<NotificationStatisticEntry> stats)
    {
      stats = this.ValidateStats(requestContext, stats);
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
        component.UpdateNotificationStatistics(stats);
    }

    public List<NotificationStatistic> QueryNotificationStatistics(
      IVssRequestContext requestContext,
      IEnumerable<NotificationStatisticsQueryConditions> queries)
    {
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
        return component.QueryNotificationStatistics(queries);
    }

    public List<TeamFoundationNotification> QueryNotifications(
      IVssRequestContext requestContext,
      IEnumerable<NotificationLookup> notificationKeys)
    {
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
        return component.QueryNotifications(notificationKeys);
    }
  }
}
