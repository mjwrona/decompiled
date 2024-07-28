// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.BaseMatcherFilter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class BaseMatcherFilter : IMatcherFilter
  {
    private const string s_area = "Notifications";
    private const string s_layer = "BaseMatcherFilter";
    private const string c_serverUrlToken = "{SERVERURL}";
    private string m_serverAccessUrl;
    protected const int DEFAULT_REGEX_TIMEOUT_SECS = 5;
    protected const long EVALUATION_TIMEOUT = 120000;
    protected const int MAX_ADDRESSES_COUNT = 100;
    internal static HashSet<string> AllowedChannelsForCustomSubscriptions = new HashSet<string>()
    {
      "EmailHtml",
      "EmailPlaintext",
      "Soap",
      "User",
      "Group"
    };

    public abstract string Matcher { get; }

    public abstract NotificationSubscription PrepareForDisplay(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags);

    public abstract void PrepareForDisplay(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter);

    public abstract Subscription PrepareForCreate(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters subscription,
      bool validate = true);

    public abstract SubscriptionLookup ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionQueryCondition condition);

    protected abstract void PostBindFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags);

    protected abstract void PreBindFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags);

    protected abstract void ValidateSubscriptionCondition(
      IVssRequestContext requestContext,
      Subscription subscription,
      bool skipWarning,
      int recipientsCount);

    public abstract SubscriptionUpdate PrepareForUpdate(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      NotificationSubscriptionUpdateParameters updateParameters);

    public virtual void PrepareForUpdate(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      SubscriptionUpdate subscriptionUpdate)
    {
      this.ReplaceEmptyIdentitiesWithDefaults(requestContext, subscriptionUpdate);
    }

    public virtual void PostBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      if (subscription.PostBindSubscriptionCompleted)
        return;
      this.UpdateIdentityFields(requestContext, subscription);
      this.SetSubscriptionFlags(requestContext, subscription);
      this.PostBindFilter(requestContext, subscription, queryFlags);
      subscription.PostBindSubscriptionCompleted = true;
    }

    public virtual void PreBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      if (subscription.PreBindSubscriptionCompleted)
        return;
      subscription.ToLegacyEventType(requestContext);
      this.ReplaceEmptyIdentitiesWithDefaults(requestContext, subscription);
      this.UpdateIdentityFields(requestContext, subscription);
      this.SetSubscriptionFlags(requestContext, subscription);
      this.PreBindFilter(requestContext, subscription, queryFlags);
      subscription.PreBindSubscriptionCompleted = true;
    }

    protected void ApplyStatusUpdates(
      NotificationSubscriptionUpdateParameters updateParameters,
      SubscriptionUpdate subscriptionUpdate)
    {
      if (!updateParameters.Status.HasValue)
        return;
      if (updateParameters.Status.Value != SubscriptionStatus.Disabled && updateParameters.Status.Value != SubscriptionStatus.DisabledByAdmin && updateParameters.Status.Value != SubscriptionStatus.DisabledFromProbation && updateParameters.Status.Value != SubscriptionStatus.Enabled && updateParameters.Status.Value != SubscriptionStatus.PendingDeletion)
        throw new ArgumentException(CoreRes.InvalidValueException((object) updateParameters.Status.Value, (object) "Status"));
      subscriptionUpdate.Status = new SubscriptionStatus?(updateParameters.Status.Value);
      subscriptionUpdate.StatusMessage = updateParameters.StatusMessage;
    }

    protected void ValidateCustomSubscriptionCreateParameters(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters)
    {
      ArgumentUtility.CheckForNull<NotificationSubscriptionCreateParameters>(createParameters, "CreateParameters");
      string type = createParameters.Channel?.Type;
      if (!string.IsNullOrEmpty(type) && !BaseMatcherFilter.AllowedChannelsForCustomSubscriptions.Contains(type))
        throw new NotificationSubscriptionChannelNotAllowedException(type);
      this.ValidateCustomNewSubscription(requestContext, createParameters.Filter, createParameters.Scope, createParameters.Channel?.Type);
    }

    public void ValidateCustomNewSubscription(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter,
      SubscriptionScope scope,
      string channel)
    {
      ArgumentUtility.CheckForNull<ISubscriptionFilter>(filter, "Filter");
      ArgumentUtility.CheckStringForNullOrEmpty(filter.Type, "FilterType");
      INotificationEventService service = requestContext.GetService<INotificationEventService>();
      if (filter.EventType == null)
        return;
      NotificationEventType eventType = service.GetEventType(requestContext, filter.EventType);
      if (eventType == null)
        throw new NotificationEventTypeNotFoundException(filter.EventType);
      if (!eventType.CustomSubscriptionsAllowed && !"ServiceHooks".Equals(channel))
        throw new NotificationEventTypeNotAllowedException(NotificationsWebApiResources.InvalidEventTypeForCustomSubscriptions((object) filter.EventType));
      if (scope != null && !eventType.SupportedScopes.Contains("project") && scope.Id != Guid.Empty && scope.Id != NotificationClientConstants.CollectionScope)
        throw new NotificationEventTypeNotSupportedScopeException(NotificationsWebApiResources.InvalidScopeForCustomSubscriptions((object) filter.EventType));
    }

    protected void ValidateCustomSubscriptionUpdateParameters(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      ArgumentUtility.CheckForNull<NotificationSubscriptionUpdateParameters>(updateParameters, "UpdateParameters");
      if (updateParameters.Channel?.Type != null && !BaseMatcherFilter.AllowedChannelsForCustomSubscriptions.Contains(updateParameters.Channel.Type))
        throw new NotificationSubscriptionChannelNotAllowedException(updateParameters.Channel.Type);
      if (updateParameters.Filter?.EventType == null)
        return;
      NotificationEventType eventType = requestContext.GetService<INotificationEventService>().GetEventType(requestContext, updateParameters.Filter.EventType);
      if (eventType == null)
        throw new NotificationEventTypeNotFoundException(updateParameters.Filter.EventType);
      if (!originalSubscription.SubscriptionFilter.Type.Equals(updateParameters.Filter.Type) && "Block".Equals(originalSubscription.SubscriptionFilter.Type, StringComparison.OrdinalIgnoreCase) && "Block".Equals(updateParameters.Filter.Type, StringComparison.OrdinalIgnoreCase))
        throw new BlockFilterUpdateNotAllowedException(NotificationsWebApiResources.InvalidFilterTypeUpdate());
      if (updateParameters.Scope != null && !eventType.SupportedScopes.Contains("project") && updateParameters.Scope.Id != Guid.Empty && updateParameters.Scope.Id != NotificationClientConstants.CollectionScope)
        throw new NotificationEventTypeNotSupportedScopeException(NotificationsWebApiResources.InvalidScopeForCustomSubscriptions((object) updateParameters.Filter.EventType));
    }

    protected int ValidateDeliveryPreferences(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity,
      string channel,
      string deliveryAddress)
    {
      int num = 0;
      switch (channel)
      {
        case "EmailHtml":
        case "EmailPlaintext":
        case "User":
          if (string.IsNullOrWhiteSpace(deliveryAddress))
          {
            if (subscriberIdentity != null && !subscriberIdentity.IsContainer && !IdentityDescriptorComparer.Instance.Equals(subscriberIdentity.Descriptor, requestContext.ServiceHost.SystemDescriptor()))
            {
              string str = (string) null;
              object obj = (object) null;
              if (subscriberIdentity.TryGetProperty("CustomNotificationAddresses", out obj))
                str = obj as string;
              if (str == null)
                str = subscriberIdentity.GetProperty<string>("Mail", string.Empty);
              if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException(CoreRes.EventSubscriptionInvalidEmailEmpty(), "preferences");
              num = 1;
              break;
            }
            break;
          }
          string[] strArray = deliveryAddress.Split(new char[2]
          {
            ';',
            ','
          }, StringSplitOptions.RemoveEmptyEntries);
          try
          {
            foreach (string address in strArray)
            {
              MailAddress mailAddress = new MailAddress(address);
            }
            if (strArray.Length == 1)
            {
              if (subscriberIdentity != null)
              {
                string notificationEmailAddress = subscriberIdentity.GetPreferredNotificationEmailAddress(requestContext);
                if (strArray[0] == notificationEmailAddress)
                  deliveryAddress = string.Empty;
              }
            }
          }
          catch (FormatException ex)
          {
            throw new ArgumentException(CoreRes.EventSubscriptionInvalidEmail((object) deliveryAddress));
          }
          num = strArray.Length;
          break;
        case "Soap":
          this.ValidationSoapEndpoint(requestContext, deliveryAddress);
          break;
        case "UserSystem":
          throw new ArgumentException(CoreRes.CantSubscribeToChannel((object) channel));
      }
      return num;
    }

    protected void ValidationSoapEndpoint(IVssRequestContext requestContext, string deliveryAddress)
    {
      ArgumentUtility.CheckForNull<string>(deliveryAddress, nameof (deliveryAddress));
      string uriString = this.ReplaceSubscriptionTokens(requestContext, deliveryAddress);
      if (string.IsNullOrEmpty(uriString) || !Uri.TryCreate(uriString, UriKind.Absolute, out Uri _))
        throw new ArgumentException(CoreRes.EventSubscriptionInvalidUri((object) deliveryAddress), nameof (deliveryAddress));
    }

    internal string ReplaceSubscriptionTokens(
      IVssRequestContext requestContext,
      string subscriptionAddress)
    {
      if (this.m_serverAccessUrl == null)
      {
        lock (this)
        {
          if (this.m_serverAccessUrl == null)
            this.m_serverAccessUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
        }
      }
      return subscriptionAddress.Replace("{SERVERURL}", this.m_serverAccessUrl);
    }

    protected void SetSubscriptionFlags(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      if (subscription.SubscriberIdentity.IsContainer)
      {
        subscription.Flags |= SubscriptionFlags.GroupSubscription;
        if (!subscription.IsTeam && subscription.SubscriberIdentity.IsTeam(requestContext))
          subscription.Flags |= SubscriptionFlags.TeamSubscription;
      }
      if (subscription.IsOptOutable())
        subscription.Flags |= SubscriptionFlags.CanOptOut;
      else
        subscription.Flags &= ~SubscriptionFlags.CanOptOut;
    }

    protected void SetSubscriptionUpdateFlags(
      Subscription originalSubscription,
      SubscriptionUpdate subscriptionUpdate)
    {
      if (subscriptionUpdate.Matcher == null && subscriptionUpdate.Address == null && subscriptionUpdate.Channel == null)
        return;
      SubscriptionFlags flags = originalSubscription.Flags;
      bool flag = false;
      string str1 = subscriptionUpdate.Matcher ?? originalSubscription.Matcher;
      string str2 = subscriptionUpdate.Address ?? originalSubscription.DeliveryAddress;
      if (subscriptionUpdate.Channel == null)
      {
        string channel = originalSubscription.Channel;
      }
      if (originalSubscription.IsContributed && (str1 == "ActorMatcher" || str1 == "ActorExpressionMatcher"))
        flag = true;
      if (!flag && originalSubscription.IsGroup && string.IsNullOrEmpty(str2) && NotificationFrameworkConstants.EmailTargetDeliveryChannels.Contains(subscriptionUpdate.Channel) && NotificationFrameworkConstants.OptOutMatcherCandidates.Contains(str1))
        flag = true;
      SubscriptionFlags subscriptionFlags = !flag ? flags & ~SubscriptionFlags.CanOptOut : flags | SubscriptionFlags.CanOptOut;
      subscriptionUpdate.Flags = new SubscriptionFlags?(subscriptionFlags);
    }

    protected void ReplaceEmptyIdentitiesWithDefaults(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (!requestContext.IsSystemContext && !IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity))
      {
        subscription.LastModifiedByIdentity = userIdentity;
        subscription.LastModifiedBy = subscription.LastModifiedByIdentity.Id;
      }
      if (!(subscription.SubscriberId == Guid.Empty))
        return;
      subscription.SubscriberIdentity = userIdentity;
      subscription.SubscriberId = subscription.SubscriberIdentity.Id;
    }

    protected void ReplaceEmptyIdentitiesWithDefaults(
      IVssRequestContext requestContext,
      SubscriptionUpdate subscriptionUpdate)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (requestContext.IsSystemContext || IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity))
        return;
      subscriptionUpdate.LastModifiedBy = new Guid?(userIdentity.Id);
    }

    protected void UpdateIdentityFields(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      if (subscription.SubscriberIdentity != null && subscription.LastModifiedByIdentity != null)
        return;
      IdentityService service = requestContext.GetService<IdentityService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid[] identityIds;
      if (!(subscription.LastModifiedBy == Guid.Empty))
        identityIds = new Guid[2]
        {
          subscription.SubscriberId,
          subscription.LastModifiedBy
        };
      else
        identityIds = new Guid[1]
        {
          subscription.SubscriberId
        };
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext1, (IList<Guid>) identityIds, QueryMembership.None, (IEnumerable<string>) null);
      subscription.SubscriberIdentity = identityList[0];
      if (subscription.SubscriberIdentity == null)
      {
        requestContext.Trace(1002110, TraceLevel.Warning, "Notifications", nameof (BaseMatcherFilter), "Subscriber Identity not found {0}", (object) subscription.SubscriberId);
        throw new IdentityNotFoundException(CoreRes.ErrorSubscriberInvalid((object) subscription.SubscriberId));
      }
      if (!(subscription.LastModifiedBy != Guid.Empty))
        return;
      subscription.LastModifiedByIdentity = identityList[1];
      if (subscription.LastModifiedByIdentity == null && subscription.LastModifiedByWillBeUsedForAuth())
      {
        requestContext.Trace(1002110, TraceLevel.Warning, "Notifications", nameof (BaseMatcherFilter), "LastModifiedBy Identity not found {0}", (object) subscription.LastModifiedBy);
        throw new IdentityNotFoundException(CoreRes.ErrorLastModifiedByInvalid((object) subscription.LastModifiedBy));
      }
    }
  }
}
