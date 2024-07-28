// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.SubscriptionExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi.Clients;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public static class SubscriptionExtensions
  {
    private static string s_layer = typeof (SubscriptionExtensions).Name;
    private static string s_area = typeof (SubscriptionExtensions).Namespace;

    public static ServiceHooksChannelMetadata ToMetadata(this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      if (subscription.ConsumerInputs != null)
      {
        subscription.ConsumerInputs.TryGetValue("resourceDetailsToSend", out str1);
        subscription.ConsumerInputs.TryGetValue("messagesToSend", out str2);
        subscription.ConsumerInputs.TryGetValue("detailedMessagesToSend", out str3);
      }
      return new ServiceHooksChannelMetadata()
      {
        PublisherId = subscription.PublisherId,
        EventType = subscription.EventType,
        ConsumerId = subscription.ConsumerId,
        ConsumerActionId = subscription.ConsumerActionId,
        ResourceVersion = subscription.ResourceVersion,
        ResourceDetailsToSend = str1,
        MessagesToSend = str2,
        DetailedMessagesToSend = str3
      };
    }

    public static string GetPublisherInput(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      string inputId,
      bool required = false)
    {
      string publisherInput = (string) null;
      if (((!subscription.PublisherInputs.TryGetValue(inputId, out publisherInput) ? 1 : (string.IsNullOrEmpty(publisherInput) ? 1 : 0)) & (required ? 1 : 0)) != 0)
        throw new ArgumentException(inputId);
      return publisherInput;
    }

    public static string GetConsumerInput(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      string inputId,
      bool required = false)
    {
      string consumerInput = (string) null;
      if (((!subscription.ConsumerInputs.TryGetValue(inputId, out consumerInput) ? 1 : (string.IsNullOrEmpty(consumerInput) ? 1 : 0)) & (required ? 1 : 0)) != 0)
        throw new ArgumentException(inputId);
      return consumerInput;
    }

    public static void SetSubscriptionUrl(
      this IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in subscriptions)
        subscription.SetSubscriptionUrl(urlHelper, requestContext);
    }

    public static void SetSubscriptionUrl(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      Guid subscriptionsLocationId = ServiceHooksPublisherApiConstants.SubscriptionsLocationId;
      if (urlHelper == null)
        return;
      subscription.Url = urlHelper.RestLink(requestContext, subscriptionsLocationId, (object) new
      {
        subscriptionId = subscription.Id
      });
      subscription.Links = ServiceHooksLinksUtility.GetSubscriptionReferenceLinks(requestContext, subscription, urlHelper);
    }

    public static void SetSubscriptionSubscriber(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      IVssRequestContext requestContext)
    {
      string input;
      Guid result;
      if (!subscription.PublisherInputs.TryGetValue("subscriberId", out input) || !Guid.TryParse(input, out result))
        return;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
      {
        result
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return;
      subscription.Subscriber = new IdentityRef()
      {
        Id = identity.Id.ToString(),
        DisplayName = identity.DisplayName,
        IsContainer = identity.IsContainer,
        UniqueName = IdentityHelper.GetUniqueName(identity)
      };
    }

    public static void OverrideServiceIdentity(
      this IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions,
      IVssRequestContext requestContext,
      ServiceHooksTimer timer = null)
    {
      INotificationSubscriptionService service1 = requestContext.GetService<INotificationSubscriptionService>();
      IdentityService service2 = requestContext.GetService<IdentityService>();
      subscriptions.OverrideServiceIdentity(requestContext, service1, service2);
    }

    public static void OverrideServiceIdentity(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      IVssRequestContext requestContext)
    {
      INotificationSubscriptionService service1 = requestContext.GetService<INotificationSubscriptionService>();
      IdentityService service2 = requestContext.GetService<IdentityService>();
      ((IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) new Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription[1]
      {
        subscription
      }).OverrideServiceIdentity(requestContext, service1, service2);
    }

    public static Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription GetHooksSubscription(
      this Microsoft.VisualStudio.Services.Notifications.Server.Subscription localSubscription)
    {
      string metadata = localSubscription.Metadata;
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription hooksSubscription = !string.IsNullOrEmpty(metadata) ? JsonConvert.DeserializeObject<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(metadata, NotificationsSerialization.JsonSerializerSettings) : throw new ArgumentException();
      hooksSubscription.IsLocal = localSubscription.IsLocalServiceHooksDelivery;
      return hooksSubscription;
    }

    public static void UpdateStatus(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription notificationSubscription)
    {
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus status1 = subscription.Status;
      Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus status2 = notificationSubscription.Status;
      if (status2 == Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.Enabled)
        SubscriptionExtensions.TraceComparison(requestContext, subscription, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.Enabled);
      else if (status2 == Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.EnabledOnProbation && (status1 == Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.Enabled || status1 == Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.OnProbation))
      {
        SubscriptionExtensions.TraceComparison(requestContext, subscription, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.OnProbation);
        subscription.Status = Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.OnProbation;
      }
      else if (status2 == Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.DisabledInactiveIdentity || status2 == Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.DisabledMissingIdentity || status2 == Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.DisabledMissingPermissions || status1 == Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledByInactiveIdentity)
      {
        SubscriptionExtensions.TraceComparison(requestContext, subscription, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledByInactiveIdentity);
        subscription.Status = Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledByInactiveIdentity;
      }
      else if (status2 == Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.Disabled || status2 == Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.DisabledByAdmin || status1 == Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledByUser)
      {
        SubscriptionExtensions.TraceComparison(requestContext, subscription, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledByUser);
        subscription.Status = Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledByUser;
      }
      else
      {
        SubscriptionExtensions.TraceComparison(requestContext, subscription, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledBySystem);
        subscription.Status = Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledBySystem;
      }
    }

    private static void TraceComparison(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus expectedStatus)
    {
      if (subscription.Status == expectedStatus)
        return;
      requestContext.Trace(1063810, TraceLevel.Info, SubscriptionExtensions.s_area, SubscriptionExtensions.s_layer, string.Format("Subscription {0} status was expected to be {1} but was actually {2}", (object) subscription.Id, (object) expectedStatus, (object) subscription.Status));
    }

    public static void UpdateStatuses(
      this IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions,
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> notificationSubscriptions)
    {
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in subscriptions)
      {
        string tfsSubId;
        if (subscription.PublisherInputs.TryGetValue("tfsSubscriptionId", out tfsSubId))
        {
          Microsoft.VisualStudio.Services.Notifications.Server.Subscription notificationSubscription = notificationSubscriptions.FirstOrDefault<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, bool>) (s => s.Tag == tfsSubId));
          if (notificationSubscription != null)
            subscription.UpdateStatus(requestContext, notificationSubscription);
        }
      }
    }

    public static bool Matches(this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription, SubscriptionsQuery query) => (1 & (query.PublisherId == null ? 1 : (subscription.PublisherId == query.PublisherId ? 1 : 0)) & (query.ConsumerId == null ? 1 : (subscription.ConsumerId == query.ConsumerId ? 1 : 0)) & (query.ConsumerActionId == null ? 1 : (subscription.ConsumerActionId == query.ConsumerActionId ? 1 : 0)) & (query.EventType == null ? 1 : (subscription.EventType == query.EventType ? 1 : 0)) & (query.PublisherInputFilters == null ? 1 : (query.PublisherInputFilters.Evaluate(subscription.PublisherInputs) ? 1 : 0)) & (query.ConsumerInputFilters == null ? 1 : (query.ConsumerInputFilters.Evaluate(subscription.ConsumerInputs) ? 1 : 0))) != 0;

    private static void OverrideServiceIdentity(
      this IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions,
      IVssRequestContext requestContext,
      INotificationSubscriptionService notificationService,
      IdentityService identityService,
      ServiceHooksTimer timer = null)
    {
      try
      {
        Dictionary<Guid, List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>> dictionary1 = new Dictionary<Guid, List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>>();
        foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription1 in subscriptions)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = identityService.ReadIdentities(requestContext, (IList<Guid>) new Guid[2]
          {
            new Guid(subscription1.CreatedBy.Id),
            new Guid(subscription1.ModifiedBy.Id)
          }, QueryMembership.None, (IEnumerable<string>) null);
          Microsoft.VisualStudio.Services.Identity.Identity identity1 = identityList[0];
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = identityList[1];
          if (!IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) identity1))
            subscription1.CreatedBy = (IdentityRef) null;
          string classification;
          if (!IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) identity2) && subscription1.PublisherInputs != null && subscription1.PublisherInputs.TryGetValue("tfsSubscriptionId", out classification))
          {
            SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(classification: classification);
            List<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> source = notificationService.QuerySubscriptions(requestContext, anyFieldLookup);
            if (source.Count > 0)
            {
              Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription2 = source.OrderByDescending<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, int>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, int>) (s => s.ID)).FirstOrDefault<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>();
              List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptionList;
              if (dictionary1.TryGetValue(subscription2.SubscriberId, out subscriptionList))
              {
                subscriptionList.Add(subscription1);
              }
              else
              {
                subscriptionList = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
                subscriptionList.Add(subscription1);
                dictionary1[subscription2.SubscriberId] = subscriptionList;
              }
            }
          }
        }
        timer?.RecordTick();
        if (dictionary1.Count <= 0)
          return;
        Dictionary<Guid, IdentityRef> dictionary2 = new Dictionary<Guid, IdentityRef>();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityService.ReadIdentities(requestContext, (IList<Guid>) dictionary1.Keys.ToArray<Guid>(), QueryMembership.None, (IEnumerable<string>) null))
        {
          if (readIdentity != null && readIdentity.Descriptor != (IdentityDescriptor) null && !string.IsNullOrEmpty(readIdentity.Descriptor.Identifier))
          {
            dictionary2[readIdentity.Id] = new IdentityRef()
            {
              Id = readIdentity.Id.ToString(),
              DisplayName = readIdentity.DisplayName,
              UniqueName = IdentityHelper.GetUniqueName(readIdentity)
            };
            break;
          }
        }
        foreach (KeyValuePair<Guid, List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>> keyValuePair in dictionary1)
        {
          IdentityRef identityRef;
          if (dictionary2.TryGetValue(keyValuePair.Key, out identityRef))
          {
            foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in keyValuePair.Value)
              subscription.ModifiedBy = identityRef;
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061170, SubscriptionExtensions.s_area, SubscriptionExtensions.s_layer, ex);
      }
    }

    private static Guid GetProjectScope(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      Guid result = Guid.Empty;
      string input;
      if (subscription.PublisherInputs.TryGetValue("projectId", out input))
        Guid.TryParse(input, out result);
      return result;
    }

    public static NotificationHttpClient GetNotificationHttpClient(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ServiceHooksPublisherService>().GetNotificationHttpClientForPublisher(requestContext, subscription.PublisherId);
    }
  }
}
