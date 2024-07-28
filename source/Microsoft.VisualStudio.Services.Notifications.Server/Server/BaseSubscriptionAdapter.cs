// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.BaseSubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class BaseSubscriptionAdapter : ISubscriptionAdapter
  {
    public abstract string SubscriptionTypeName { get; }

    public abstract string FilterType { get; }

    public SubscriptionScope CurrentScope { get; set; }

    public abstract ISubscriptionFilter CreateSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags);

    public virtual void InitializeAdapter(
      IVssRequestContext requestContext,
      string eventType,
      SubscriptionScope scopeId)
    {
      this.CurrentScope = scopeId;
    }

    public virtual void ApplyFilterUpdatesToSubscription(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      if (updateParameters.Filter == null)
        return;
      subscriptionToPatch.Matcher = !(updateParameters.Filter.Type == "Actor") ? this.GetMatcher(requestContext, subscriptionToPatch.SubscriptionFilter.EventType) : "ActorMatcher";
      if (string.IsNullOrEmpty(updateParameters.Filter.EventType))
        return;
      updateParameters.Filter.EventType = EventTypeMapper.ToLegacy(requestContext, updateParameters.Filter.EventType);
      subscriptionToPatch.EventTypeName = updateParameters.Filter.EventType;
    }

    public abstract string GetMatcher(IVssRequestContext requestContext, string eventType);

    public virtual EventSerializerType GetSerializationFormatForEvent(
      IVssRequestContext requestContext,
      string eventType)
    {
      return requestContext.GetService<INotificationEventService>().GetSerializationFormatForEvent(requestContext, eventType);
    }

    public virtual bool AllowDuplicateSubscriptions() => true;

    public virtual void ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionLookup lookup,
      ISubscriptionFilter filter)
    {
    }

    public virtual void PostBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
    }

    public virtual void PreBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
    }

    public virtual Condition GetOptimizedCondition(Token fieldName, byte op, Token target) => (Condition) null;

    public virtual List<NotificationEventRole> GetRoles(IVssRequestContext requestContext) => new List<NotificationEventRole>();

    public virtual void PrepareForDisplay(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter)
    {
    }

    protected string GetTeamSubscriptionUrl(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      try
      {
        if (subscription.IsTeam)
        {
          if (requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
          {
            Guid guid = subscription.ScopeId;
            string empty;
            if (!guid.Equals(NotificationClientConstants.CollectionScope))
            {
              guid = Guid.Empty;
              if (!guid.Equals(subscription.ScopeId))
              {
                guid = subscription.ScopeId;
                empty = guid.ToString();
                goto label_6;
              }
            }
            empty = string.Empty;
label_6:
            string str1 = empty;
            Guid projectId = subscription.ProjectId;
            guid = Guid.Empty;
            string str2;
            if (guid.Equals(subscription.ProjectId))
            {
              str2 = str1;
            }
            else
            {
              guid = subscription.ProjectId;
              str2 = guid.ToString();
            }
            string str3 = str2;
            string str4 = requestContext.GetService<IContributionRoutingService>().RouteUrl(requestContext, "ms.vss-notifications-web.team-notifications-route", new RouteValueDictionary()
            {
              {
                "project",
                (object) str3
              },
              {
                "team",
                (object) subscription.SubscriberId
              }
            });
            if (!string.IsNullOrEmpty(str4))
            {
              StringBuilder stringBuilder = new StringBuilder(str4);
              stringBuilder.Append("?");
              stringBuilder.AppendFormat("{0}={1}", (object) "subscriptionId", (object) subscription.SubscriptionId);
              NotificationEventType eventType = requestContext.GetService<INotificationEventService>().GetEventType(requestContext, subscription.SubscriptionFilter.EventType);
              if (eventType != null && eventType.EventPublisher != null)
                stringBuilder.AppendFormat("&{0}={1}", (object) "publisherId", (object) eventType.EventPublisher.Id);
              return stringBuilder.ToString();
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return string.Empty;
    }

    protected NotificationSubscription GetNotificationSubsriptionById(
      IVssRequestContext requestContext,
      int subscriptionId)
    {
      Subscription subscriptionById = this.GetSubscriptionById(requestContext, subscriptionId);
      return this.ToNotificationSubscription(requestContext, subscriptionById, SubscriptionQueryFlags.IncludeFilterDetails);
    }

    protected Subscription GetSubscriptionById(
      IVssRequestContext requestContext,
      int subscriptionId)
    {
      return requestContext.GetService<INotificationSubscriptionService>().GetSubscription(requestContext, subscriptionId);
    }

    public virtual Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscription notificationSubscription)
    {
      int result = -1;
      string str = (string) null;
      Guid guid1 = Guid.Empty;
      Guid guid2 = Guid.Empty;
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      SubscriptionFlags flags = notificationSubscription.Flags;
      if (notificationSubscription.Id != null && !int.TryParse(notificationSubscription.Id, out result))
      {
        str = notificationSubscription.Id;
        flags |= SubscriptionFlags.GroupSubscription | SubscriptionFlags.ContributedSubscription;
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity2;
      if (!flags.HasFlag((Enum) SubscriptionFlags.ContributedSubscription))
      {
        identity2 = notificationSubscription.Subscriber == null ? requestContext.GetUserIdentity() : notificationSubscription.Subscriber.ToVsIdentity(requestContext);
        guid1 = identity2.Id;
        if (notificationSubscription.LastModifiedBy != null)
        {
          identity1 = notificationSubscription.LastModifiedBy.ToVsIdentity(requestContext);
          guid2 = identity1.Id;
        }
      }
      else
        identity2 = requestContext.GetService<INotificationSubscriptionServiceInternal>().GetValidUsersGroup(requestContext);
      return new Subscription((ISubscriptionAdapter) this)
      {
        ID = result,
        EventTypeName = notificationSubscription.Filter?.EventType,
        ContributionId = str,
        Description = notificationSubscription.Description,
        Status = notificationSubscription.Status,
        StatusMessage = notificationSubscription.StatusMessage,
        ModifiedTime = notificationSubscription.ModifiedDate,
        SubscriberIdentity = identity2,
        SubscriberId = guid1,
        LastModifiedByIdentity = identity1,
        LastModifiedBy = guid2,
        Channel = notificationSubscription.Channel?.Type,
        DeliveryAddress = SubscriptionChannel.GetAddress(notificationSubscription.Channel),
        Flags = flags,
        SubscriptionScope = notificationSubscription.Scope,
        SubscriptionFilter = notificationSubscription.Filter
      };
    }

    public virtual Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters)
    {
      int num = -1;
      Guid empty = Guid.Empty;
      Microsoft.VisualStudio.Services.Identity.Identity identity = createParameters.Subscriber == null ? requestContext.GetUserIdentity() : createParameters.Subscriber.ToVsIdentity(requestContext);
      Guid id = identity.Id;
      return new Subscription((ISubscriptionAdapter) this)
      {
        ID = num,
        EventTypeName = createParameters.Filter?.EventType,
        Description = createParameters.Description,
        SubscriberIdentity = identity,
        SubscriberId = id,
        Channel = createParameters.Channel?.Type,
        DeliveryAddress = SubscriptionChannel.GetAddress(createParameters.Channel),
        SubscriptionScope = createParameters.Scope,
        SubscriptionFilter = createParameters.Filter
      };
    }

    public virtual NotificationSubscription ToNotificationSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      SubscriptionScope subscriptionScope1 = new SubscriptionScope();
      subscriptionScope1.Id = subscription.ScopeId;
      SubscriptionScope subscriptionScope2 = subscriptionScope1;
      NotificationSubscription notificationSubscription1 = new NotificationSubscription();
      notificationSubscription1.Id = subscription.SubscriptionId;
      notificationSubscription1.ExtendedProperties = subscription.ExtendedProperties;
      Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity = subscription.SubscriberIdentity;
      notificationSubscription1.Subscriber = subscriberIdentity != null ? subscriberIdentity.ToIdentityRef() : (IdentityRef) null;
      Microsoft.VisualStudio.Services.Identity.Identity modifiedByIdentity = subscription.LastModifiedByIdentity;
      notificationSubscription1.LastModifiedBy = modifiedByIdentity != null ? modifiedByIdentity.ToIdentityRef() : (IdentityRef) null;
      notificationSubscription1.Channel = SubscriptionChannel.Create(subscription.Channel, subscription.DeliveryAddress);
      notificationSubscription1.Scope = subscriptionScope2;
      notificationSubscription1.ModifiedDate = subscription.ModifiedTime;
      notificationSubscription1.Description = subscription.Description;
      notificationSubscription1.Status = subscription.Status;
      notificationSubscription1.StatusMessage = subscription.StatusMessage;
      notificationSubscription1.Flags = subscription.Flags;
      notificationSubscription1.Permissions = subscription.Permissions;
      notificationSubscription1.Filter = subscription.SubscriptionFilter;
      notificationSubscription1.UniqueId = subscription.UniqueId.ToString("D");
      notificationSubscription1.Diagnostics = subscription.Diagnostics;
      NotificationSubscription notificationSubscription2 = notificationSubscription1;
      subscription.ToContributedEventType(requestContext);
      if (subscription.IsOptOutable())
      {
        notificationSubscription2.UserSettings = new SubscriptionUserSettings()
        {
          OptedOut = subscription.UserSettingsOptedOut
        };
        notificationSubscription2.AdminSettings = new SubscriptionAdminSettings()
        {
          BlockUserOptOut = subscription.AdminSettingsBlockUserOptOut
        };
      }
      notificationSubscription2.Url = NotificationSubscriptionService.GetSubscriptionRestURL(requestContext, subscription.SubscriptionId);
      if (!string.IsNullOrEmpty(subscription.DeliveryAddress) && (subscription.IsEmailDelivery || subscription.IsSoapDelivery))
        ((SubscriptionChannelWithAddress) notificationSubscription2.Channel).UseCustomAddress = true;
      return notificationSubscription2;
    }

    protected void PopulateNotificationSubscriptionLinks(
      IVssRequestContext requestContext,
      Subscription subscription,
      NotificationSubscription notificationSubscription)
    {
      if (subscription.SubscriberIdentity.IsContainer && !subscription.Flags.HasFlag((Enum) SubscriptionFlags.ContributedSubscription))
      {
        string teamSubscriptionUrl = this.GetTeamSubscriptionUrl(requestContext, subscription);
        if (string.IsNullOrEmpty(teamSubscriptionUrl))
          return;
        notificationSubscription.Links.AddLink("edit", teamSubscriptionUrl);
        notificationSubscription.Links.AddLink("team", teamSubscriptionUrl);
      }
      else
      {
        string viewSubscriptionUrl = this.GetViewSubscriptionUrl(requestContext, subscription);
        if (string.IsNullOrEmpty(viewSubscriptionUrl))
          return;
        notificationSubscription.Links.AddLink("edit", viewSubscriptionUrl);
      }
    }

    protected void ValidateSubscriptionPatch(
      Subscription originalSubscription,
      NotificationSubscription patchSubscription)
    {
      if (this.FilterType != patchSubscription.Filter.Type)
        throw new ArgumentException(CoreRes.FilterTypeCantBeChanged((object) originalSubscription.ID, (object) this.FilterType, (object) patchSubscription.Filter.Type));
    }

    protected void VerifyFieldValueNotChanged(
      string origFieldValue,
      string newFieldValue,
      string fieldName,
      string subscriptionId)
    {
      if (newFieldValue != null && newFieldValue != origFieldValue)
        throw new ArgumentException(CoreRes.FieldCannotBeUpdated((object) fieldName, (object) subscriptionId));
    }

    protected void VerifyFieldIsNull(object field, string fieldName, int subscriptionId)
    {
      if (field != null)
        throw new ArgumentException(CoreRes.FieldCannotBeUpdated((object) fieldName, (object) subscriptionId));
    }

    protected void PublishCiData(
      IVssRequestContext requestContext,
      CustomerIntelligenceData ciData,
      string action,
      string layer)
    {
      try
      {
        ciData.Add(nameof (action), action);
        ciData.Add(nameof (layer), layer);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Notifications", "Follows", ciData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15092000, "Follows", "FollowsService", ex);
      }
    }

    internal virtual string GetViewSubscriptionUrl(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      string notificationsUrl = NotificationSubscriptionService.GetMyNotificationsUrl(requestContext, subscription);
      return !string.IsNullOrEmpty(notificationsUrl) ? new Uri(notificationsUrl).AppendQuery("action", "view").ToString() : string.Empty;
    }
  }
}
