// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherNotificationsQueryController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "NotificationsQuery")]
  [ClientGroupByResource("notifications")]
  public class HooksPublisherNotificationsQueryController : ServiceHooksPublisherControllerBase
  {
    private static readonly string s_layer = typeof (HooksPublisherNotificationsQueryController).Name;
    private static readonly string s_area = typeof (HooksPublisherNotificationsQueryController).Namespace;

    [HttpPost]
    public NotificationsQuery QueryNotifications(NotificationsQuery query)
    {
      ArgumentUtility.CheckForNull<NotificationsQuery>(query, nameof (query));
      IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> localSubscriptions = this.GetLocalSubscriptions(this.TfsRequestContext, (IEnumerable<Guid>) query.SubscriptionIds);
      if (localSubscriptions.Any<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>())
        this.FilterLocalSubscriptionIdsFromQuery(query, localSubscriptions);
      NotificationsQuery notificationsQuery = query;
      if (!localSubscriptions.Any<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>() || query.SubscriptionIds.Count > 0)
        notificationsQuery = this.GetHooksService().QueryNotifications(this.TfsRequestContext, query);
      bool? includeDetails;
      if (localSubscriptions.Any<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>())
      {
        includeDetails = query.IncludeDetails;
        if (includeDetails.HasValue)
        {
          includeDetails = query.IncludeDetails;
          bool flag = false;
          if (includeDetails.GetValueOrDefault() == flag & includeDetails.HasValue)
          {
            List<NotificationSummary> localSummaries = this.GetLocalSummaries(this.TfsRequestContext, localSubscriptions);
            if (notificationsQuery.Summary == null)
              notificationsQuery.Summary = (IList<NotificationSummary>) localSummaries;
            else
              notificationsQuery.Summary.AddRange<NotificationSummary, IList<NotificationSummary>>((IEnumerable<NotificationSummary>) localSummaries);
            return notificationsQuery;
          }
        }
        List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> localResults = this.GetLocalResults(this.TfsRequestContext, localSubscriptions.Select<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, Guid>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, Guid>) (ls => ls.UniqueId)));
        if (notificationsQuery.Results == null)
          notificationsQuery.Results = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) localResults;
        else
          notificationsQuery.Results.AddRange<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification, IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>>((IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) localResults);
        if (notificationsQuery.AssociatedSubscriptions == null)
        {
          notificationsQuery.AssociatedSubscriptions = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
          foreach (Microsoft.VisualStudio.Services.Notifications.Server.Subscription localSubscription in localSubscriptions)
          {
            Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription hooksSubscription = localSubscription.GetHooksSubscription();
            if (hooksSubscription != null)
              notificationsQuery.AssociatedSubscriptions.Add(hooksSubscription);
          }
        }
      }
      if (notificationsQuery.AssociatedSubscriptions == null)
        notificationsQuery.Results = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) null;
      else if (notificationsQuery.Results != null)
      {
        Dictionary<string, ServiceHooksPublisher> dictionary = this.FindPublishers(query.PublisherId).ToDictionary<ServiceHooksPublisher, string, ServiceHooksPublisher>((Func<ServiceHooksPublisher, string>) (k => k.Id), (Func<ServiceHooksPublisher, ServiceHooksPublisher>) (v => v));
        HashSet<Guid> subscriptionIdsWithoutPermission = new HashSet<Guid>();
        HashSet<Guid> subscriptionsToScrub = new HashSet<Guid>();
        foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription associatedSubscription in (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) notificationsQuery.AssociatedSubscriptions)
        {
          ServiceHooksPublisher publisher;
          if (!dictionary.TryGetValue(associatedSubscription.PublisherId, out publisher))
            subscriptionIdsWithoutPermission.Add(associatedSubscription.Id);
          else if (!publisher.CheckPermission(this.TfsRequestContext, associatedSubscription, false))
          {
            subscriptionIdsWithoutPermission.Add(associatedSubscription.Id);
          }
          else
          {
            includeDetails = query.IncludeDetails;
            if (includeDetails.HasValue)
            {
              includeDetails = query.IncludeDetails;
              if (includeDetails.Value && !this.CanUserViewDiagnostics(this.TfsRequestContext, associatedSubscription, publisher))
                subscriptionsToScrub.Add(associatedSubscription.Id);
            }
          }
        }
        if (subscriptionIdsWithoutPermission.Count > 0)
          notificationsQuery.AssociatedSubscriptions = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) notificationsQuery.AssociatedSubscriptions.Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, bool>) (s => !subscriptionIdsWithoutPermission.Contains(s.Id))).ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        List<string> eventTypes = this.GetUserEventTypes();
        notificationsQuery.AssociatedSubscriptions = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(notificationsQuery.AssociatedSubscriptions.Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, bool>) (s => eventTypes.Contains(s.EventType))));
        HashSet<Guid> subscriptionIds = new HashSet<Guid>(notificationsQuery.AssociatedSubscriptions.Select<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, Guid>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, Guid>) (s => s.Id)));
        notificationsQuery.Results = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) notificationsQuery.Results.Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification, bool>) (qr => subscriptionIds.Contains(qr.SubscriptionId))).ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>();
        this.ScrubNotificationHistory(this.TfsRequestContext, notificationsQuery, subscriptionsToScrub);
      }
      return notificationsQuery;
    }

    private void FilterLocalSubscriptionIdsFromQuery(
      NotificationsQuery query,
      IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> localSubscriptions)
    {
      int num1 = 0;
      int num2 = localSubscriptions.Count<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>();
      for (int index = query.SubscriptionIds.Count - 1; index >= 0 && num1 < num2; --index)
      {
        Guid subscriptionId = query.SubscriptionIds[index];
        foreach (Microsoft.VisualStudio.Services.Notifications.Server.Subscription localSubscription in localSubscriptions)
        {
          if (localSubscription.UniqueId == subscriptionId)
          {
            query.SubscriptionIds.RemoveAt(index);
            ++num1;
            break;
          }
        }
      }
    }

    private List<NotificationSummary> GetLocalSummaries(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> localSubscriptions)
    {
      NotificationStatisticsQuery notificationStatisticsQuery = new NotificationStatisticsQuery();
      notificationStatisticsQuery.Conditions = new List<NotificationStatisticsQueryConditions>();
      Dictionary<string, NotificationSummary> dictionary = new Dictionary<string, NotificationSummary>();
      DateTime dateTime = DateTime.UtcNow.AddDays(-14.0);
      foreach (Microsoft.VisualStudio.Services.Notifications.Server.Subscription localSubscription in localSubscriptions)
      {
        NotificationSummary notificationSummary = new NotificationSummary()
        {
          SubscriptionId = localSubscription.UniqueId,
          Results = (IList<NotificationResultsSummaryDetail>) new List<NotificationResultsSummaryDetail>()
          {
            new NotificationResultsSummaryDetail()
            {
              Result = NotificationResult.Succeeded,
              NotificationCount = 0
            },
            new NotificationResultsSummaryDetail()
            {
              Result = NotificationResult.Failed,
              NotificationCount = 0
            }
          }
        };
        string key = localSubscription.UniqueId.ToString("D");
        dictionary[key] = notificationSummary;
        notificationStatisticsQuery.Conditions.Add(new NotificationStatisticsQueryConditions()
        {
          Path = key,
          Type = new NotificationStatisticType?(NotificationStatisticType.NotificationBySubscription),
          StartDate = new DateTime?(dateTime)
        });
        notificationStatisticsQuery.Conditions.Add(new NotificationStatisticsQueryConditions()
        {
          Path = key,
          Type = new NotificationStatisticType?(NotificationStatisticType.NotificationFailureBySubscription),
          StartDate = new DateTime?(dateTime)
        });
      }
      foreach (NotificationStatistic notificationStatistic in requestContext.GetService<IEventNotificationServiceInternal>().QueryNotificationStatistics(this.TfsRequestContext, (IEnumerable<NotificationStatisticsQueryConditions>) notificationStatisticsQuery.Conditions))
      {
        if (dictionary.ContainsKey(notificationStatistic.Path))
        {
          if (notificationStatistic.Type == NotificationStatisticType.NotificationBySubscription)
            dictionary[notificationStatistic.Path].Results[0].NotificationCount += notificationStatistic.HitCount;
          else if (notificationStatistic.Type == NotificationStatisticType.NotificationFailureBySubscription)
          {
            dictionary[notificationStatistic.Path].Results[0].NotificationCount -= notificationStatistic.HitCount;
            dictionary[notificationStatistic.Path].Results[1].NotificationCount += notificationStatistic.HitCount;
          }
        }
      }
      return dictionary.Values.ToList<NotificationSummary>();
    }

    private List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> GetLocalResults(
      IVssRequestContext requestContext,
      IEnumerable<Guid> hooksIds)
    {
      IEventNotificationService service = requestContext.GetService<IEventNotificationService>();
      List<NotificationLookup> notificationLookupList = new List<NotificationLookup>(hooksIds.Select<Guid, NotificationLookup>((Func<Guid, NotificationLookup>) (id => new NotificationLookup(id))));
      IVssRequestContext requestContext1 = requestContext;
      List<NotificationLookup> notificationKeys = notificationLookupList;
      List<TeamFoundationNotification> foundationNotificationList = service.QueryNotifications(requestContext1, (IEnumerable<NotificationLookup>) notificationKeys);
      List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> localResults = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>();
      foreach (TeamFoundationNotification foundationNotification in foundationNotificationList)
      {
        if (foundationNotification != null && foundationNotification.Result != null)
        {
          Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification = JsonConvert.DeserializeObject<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>(foundationNotification.Result, NotificationsSerialization.JsonSerializerSettings);
          if (foundationNotification.ResultDetail != null)
          {
            NotificationDetails notificationDetails = JsonConvert.DeserializeObject<NotificationDetails>(foundationNotification.ResultDetail, NotificationsSerialization.JsonSerializerSettings);
            notification.Details = notificationDetails;
          }
          localResults.Add(notification);
        }
      }
      return localResults;
    }
  }
}
