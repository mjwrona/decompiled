// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationSubscriptionService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationSubscriptionService : 
    INotificationSubscriptionServiceInternal,
    INotificationSubscriptionService,
    IVssFrameworkService
  {
    private static Dictionary<string, string> s_filterMatcherMap = new Dictionary<string, string>()
    {
      {
        "Expression",
        "PathMatcher"
      },
      {
        "Artifact",
        "FollowsMatcher"
      },
      {
        "Actor",
        "ActorMatcher"
      },
      {
        "Block",
        "BlockMatcher"
      }
    };
    private IDisposableReadOnlyList<INotificationsSystemEventFactory> m_eventFactoryImpls;
    private Microsoft.VisualStudio.Services.Identity.Identity m_projectCollectionValidUsers;
    private JsonSerializer s_jsonSerializer;
    private const string s_area = "Notifications";
    private const string s_layer = "NotificationSubscriptionService";
    private ConcurrentDictionary<Guid, JobRegisteredStatus> m_jobRegistrations = new ConcurrentDictionary<Guid, JobRegisteredStatus>();
    private int m_jobRegisteredCheckFrequency = 300000;
    private const string c_serverUrlToken = "{SERVERURL}";
    private const int c_defaultRegexTimeoutSecs = 5;
    private HashSet<string> m_supportedEventTypes = new HashSet<string>();
    private static readonly string s_subscriberToken = "$SUBSCRIBER:";
    private static readonly string s_subscriberTokenFormatString = NotificationSubscriptionService.s_subscriberToken + "{0}";
    internal static readonly string s_contributedSubscriptionsDataName = "Microsoft.VisualStudio.Services.Notifications.Server.ContributedSubscriptions";
    private static readonly string s_subscriptionTemplatesDataName = "Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionTemplates";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.m_jobRegisteredCheckFrequency = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (FrameworkServerConstants.NotificationRootPath + "/**"))["JobRegisteredCheckFrequency"].GetValue<int>(this.m_jobRegisteredCheckFrequency);
      this.m_eventFactoryImpls = requestContext.GetExtensions<INotificationsSystemEventFactory>(ExtensionLifetime.Service);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public NotificationSubscription CreateSubscription(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters)
    {
      Subscription subscription = this.PrepareForCreate(requestContext, createParameters, true);
      ISubscriptionAdapter adapter = SubscriptionAdapterFactory.CreateAdapter(requestContext, createParameters.Filter, createParameters.Scope);
      int subscriptionInternal = this.CreateSubscriptionInternal(requestContext, subscription, adapter.AllowDuplicateSubscriptions());
      return this.GetNotificationSubscription(requestContext, subscriptionInternal.ToString(), SubscriptionQueryFlags.IncludeFilterDetails);
    }

    public int CreateSubscription(IVssRequestContext requestContext, Subscription subscription)
    {
      if ("PathMatcher".Equals(subscription.Matcher))
        subscription.Matcher = NotificationSubscriptionService.GetPathMatcherForEvent(requestContext, subscription.SubscriptionFilter.EventType);
      IMatcherFilter matcherFilter = MatcherFilterFactory.GetMatcherFilter(requestContext, subscription.Matcher);
      matcherFilter.ValidateCustomNewSubscription(requestContext, subscription.SubscriptionFilter, subscription.SubscriptionScope, subscription.Channel);
      matcherFilter.PreBindSubscription(requestContext, subscription);
      return this.CreateSubscriptionInternal(requestContext, subscription, true);
    }

    private int CreateSubscriptionInternal(
      IVssRequestContext requestContext,
      Subscription subscription,
      bool allowDuplicateSubscriptions)
    {
      this.ValidateSubscription(requestContext, subscription);
      if (subscription.Channel.Equals("MessageQueue"))
        throw new NotificationSubscriptionChannelNotAllowedException("MessageQueue");
      if (!this.CreateOrUpdateSubscriptionAllowed(requestContext, subscription.SubscriberIdentity, subscription.Matcher))
        throw new UnauthorizedAccessException(CoreRes.UnauthorizedCreateSubscription());
      int subscriptionInternal;
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
        subscriptionInternal = component.SubscribeEvent(subscription, allowDuplicateSubscriptions);
      if (subscription.SubscriberIdentity.IsContainer)
        NotificationSubscriptionSecurityUtils.SetSubscriberAccessControlEntry(requestContext, subscription.SubscriberIdentity);
      Subscription subscription1 = (Subscription) subscription.Clone();
      subscription1.ID = subscriptionInternal;
      NotificationAuditing.PublishCreateSubscriptionEvent(requestContext, subscription1);
      return subscriptionInternal;
    }

    public bool HasPermissions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      int permission)
    {
      return NotificationSubscriptionSecurityUtils.HasPermissions(requestContext, identity, permission);
    }

    public bool CallerHasAdminPermissions(IVssRequestContext requestContext) => NotificationSubscriptionSecurityUtils.CallerHasAdminPermissions(requestContext, 2);

    public bool CallerHasManageSubscriptionsPermission(
      IVssRequestContext requestContext,
      IdentityRef subscriber)
    {
      Microsoft.VisualStudio.Services.Identity.Identity vsIdentity = subscriber.ToVsIdentity(requestContext);
      return NotificationSubscriptionSecurityUtils.HasPermissions(requestContext, vsIdentity, 2);
    }

    private Subscription PrepareForCreate(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters,
      bool validate)
    {
      ArgumentUtility.CheckForNull<NotificationSubscriptionCreateParameters>(createParameters, nameof (createParameters));
      return MatcherFilterFactory.GetMatcherFilter(requestContext, createParameters.Filter).PrepareForCreate(requestContext, createParameters, validate);
    }

    private void ValidateSubscription(IVssRequestContext requestContext, Subscription subscription)
    {
      ArgumentUtility.CheckForNull<string>(subscription.Channel, "channel");
      this.VerifyEventType(requestContext, subscription);
      this.VerifyMatcher(requestContext, subscription);
      if (subscription.SubscriptionFilter.EventType == null)
        return;
      this.VerifyMatcherCompatibleWithEvent(requestContext, subscription.Matcher, subscription.SubscriptionFilter.EventType);
    }

    private void VerifyEventType(IVssRequestContext requestContext, Subscription subscription)
    {
      if (subscription.SubscriptionFilter.EventType != null && !this.VerifyEventType(requestContext, subscription.SubscriptionFilter.EventType))
        throw new EventTypeDoesNotExistException(subscription.SubscriptionFilter.EventType);
    }

    private void VerifyMatcher(IVssRequestContext requestContext, Subscription subscription) => this.VerifyMatcher(requestContext, subscription.Matcher);

    private void VerifyMatcher(IVssRequestContext requestContext, string matcher)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(matcher, nameof (matcher));
      if (matcher != null)
      {
        switch (matcher.Length)
        {
          case 12:
            switch (matcher[0])
            {
              case 'A':
                if (matcher == "ActorMatcher")
                  return;
                break;
              case 'B':
                if (matcher == "BlockMatcher")
                  return;
                break;
              case 'X':
                if (matcher == "XPathMatcher")
                  return;
                break;
            }
            break;
          case 14:
            if (matcher == "FollowsMatcher")
              return;
            break;
          case 15:
            if (matcher == "JsonPathMatcher")
              return;
            break;
          case 21:
            if (matcher == "PathExpressionMatcher")
              return;
            break;
          case 22:
            switch (matcher[1])
            {
              case 'c':
                if (matcher == "ActorExpressionMatcher")
                  return;
                break;
              case 'u':
                if (matcher == "AuditExpressionMatcher")
                  return;
                break;
            }
            break;
        }
      }
      throw new MatcherNotsupportedException(matcher);
    }

    private void VerifyMatcherCompatibleWithEvent(
      IVssRequestContext requestContext,
      string matcherTo,
      string eventType)
    {
      bool flag;
      if (flag = !string.IsNullOrEmpty(matcherTo) && (!string.IsNullOrEmpty(eventType) || NotificationSubscriptionService.IsEventTypeRequiredMatcher(matcherTo)))
      {
        flag = NotificationSubscriptionService.IsActorMatcher(matcherTo);
        if (!flag)
        {
          string pathMatcherForEvent = NotificationSubscriptionService.GetPathMatcherForEvent(requestContext, eventType);
          flag = NotificationSubscriptionService.IsXPathCompatibleMatcher(matcherTo) && NotificationSubscriptionService.IsXPathCompatibleMatcher(pathMatcherForEvent) || NotificationSubscriptionService.IsJsonPathCompatibleMatcher(matcherTo) && NotificationSubscriptionService.IsJsonPathCompatibleMatcher(pathMatcherForEvent);
        }
      }
      if (!flag)
        throw new MatcherEventTypeCombinationNotSupportedException(matcherTo, eventType);
    }

    public SubscriptionUserSettings UpdateSubscriptionUserSettings(
      IVssRequestContext requestContext,
      string subscriptionId,
      Guid userId,
      SubscriptionUserSettings userSettings)
    {
      if (userId.Equals(Guid.Empty))
        userId = requestContext.GetUserId();
      Subscription subscription = this.ValidateUpdateUserSettingsParameters(requestContext, subscriptionId, new Guid?(userId), userSettings);
      NotificationQueryCondition notificationQueryCondition = (NotificationQueryCondition) null;
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
      {
        if (userSettings.OptedOut)
        {
          List<DefaultSubscriptionAdminCandidate> subscriptionsAdminDisabled = component.GetDefaultSubscriptionsAdminDisabled(new HashSet<string>()
          {
            subscriptionId
          });
          if (subscriptionsAdminDisabled.Count<DefaultSubscriptionAdminCandidate>() > 0 && subscriptionsAdminDisabled[0].BlockUserDisable)
            throw new SubscriptionOptOutBlocked(subscriptionId);
          notificationQueryCondition = new NotificationQueryCondition()
          {
            SubscriptionId = subscriptionId,
            Subscriber = userId
          };
        }
        component.UpdateDefaultSubscriptionsUserEnabled(userId, subscriptionId, !userSettings.OptedOut);
      }
      if (notificationQueryCondition != null)
        requestContext.GetService<IEventNotificationServiceInternal>().SuspendUnprocessedNotifications(requestContext, new List<NotificationQueryCondition>()
        {
          notificationQueryCondition
        }, false);
      NotificationAuditing.PublishUpdateSubscriptionUserSettingsEvent(requestContext, subscription, userSettings);
      return userSettings;
    }

    private Subscription ValidateUpdateUserSettingsParameters(
      IVssRequestContext requestContext,
      string subscriptionId,
      Guid? userId,
      SubscriptionUserSettings userSettings)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(subscriptionId, nameof (subscriptionId));
      ArgumentUtility.CheckForNull<SubscriptionUserSettings>(userSettings, nameof (userSettings));
      Subscription subscription = this.GetSubscription(requestContext, subscriptionId, SubscriptionQueryFlags.IncludeInvalidSubscriptions | SubscriptionQueryFlags.IncludeDeletedSubscriptions);
      Microsoft.VisualStudio.Services.Identity.Identity subscriptionIdentity = requestContext.GetUserIdentity();
      if (userId.HasValue && !requestContext.GetUserId().Equals(userId.Value))
        subscriptionIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
        {
          userId.Value
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (subscriptionIdentity == null || !NotificationSubscriptionSecurityUtils.HasPermissions(requestContext, subscriptionIdentity, 2))
        throw new UnauthorizedAccessException(CoreRes.SubscriptionOptOutUnAuth());
      if (!subscription.IsGroup)
        throw new SubscriptionOptOutNotSupported(CoreRes.SubscriptionOptOutError());
      if (!subscription.IsOptOutable())
        throw new SubscriptionOptOutNotSupported(CoreRes.SubscriptionNotOptOutable());
      return !subscription.IsSystem ? subscription : throw new SubscriptionOptOutNotSupported(CoreRes.SystemSubscriptionNotOptOutable());
    }

    public SubscriptionDiagnostics UpdateSubscriptionDiagnostics(
      IVssRequestContext requestContext,
      string subscriptionId,
      UpdateSubscripitonDiagnosticsParameters updateParameters)
    {
      this.ValidateUpdateSubscripitonDiagnosticsParameters(requestContext, updateParameters);
      Subscription subscription = this.GetSubscription(requestContext, subscriptionId, SubscriptionQueryFlags.None);
      if (!this.CreateOrUpdateSubscriptionAllowed(requestContext, subscription.SubscriberIdentity, subscription.Matcher))
        throw new UnauthorizedAccessException(CoreRes.UnauthorizedUpdateSubscription());
      subscription.UpdateDiagnostics(updateParameters);
      this.UpdateSubscriptionDiagnostics(requestContext, (IEnumerable<Subscription>) new Subscription[1]
      {
        subscription
      });
      return subscription.Diagnostics;
    }

    private void ValidateUpdateSubscripitonDiagnosticsParameters(
      IVssRequestContext requestContext,
      UpdateSubscripitonDiagnosticsParameters updateParameters)
    {
      ArgumentUtility.CheckForNull<UpdateSubscripitonDiagnosticsParameters>(updateParameters, nameof (updateParameters));
      if (updateParameters.DeliveryResults == null && updateParameters.EvaluationTracing == null && updateParameters.DeliveryTracing == null)
        throw new ArgumentException(CoreRes.InvalidUpdateTracingParametersEmpty());
    }

    public void DeleteSubscription(IVssRequestContext requestContext, string subscriptionId)
    {
      int result;
      if (!int.TryParse(subscriptionId, out result))
        throw new NotSupportedException(CoreRes.SubscriptionDoesNotSupportOperation((object) subscriptionId, (object) "Delete"));
      this.DeleteSubscription(requestContext, result);
    }

    public void DeleteSubscription(IVssRequestContext requestContext, int subscriptionId)
    {
      Subscription subscription = this.GetSubscription(requestContext, subscriptionId, SubscriptionQueryFlags.IncludeInvalidSubscriptions);
      this.UpdateSubscriptionStatus(requestContext, subscription, SubscriptionStatus.PendingDeletion, CoreRes.SubscriptionPendingDeletion(), true);
      NotificationAuditing.PublishDeleteSubscriptionEvent(requestContext, subscription);
    }

    public void JailSubscription(
      IVssRequestContext requestContext,
      NotificationStatistic stat,
      DateTime jailOnlyfNewerThanDate,
      int jailSubscriptionUserThreshold,
      int jailSubscriptionServiceHooksThreshold,
      bool auditOnly = false)
    {
      int result;
      if (!int.TryParse(stat.Path, out result))
      {
        requestContext.Trace(1002913, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), "Skipping Jailing: " + stat.Path + " due to invalid Id");
      }
      else
      {
        Subscription subscription = this.GetSubscription(requestContext, result, SubscriptionQueryFlags.IncludeDeletedSubscriptions);
        if (subscription.Status < SubscriptionStatus.Enabled || !(subscription.ModifiedTime >= jailOnlyfNewerThanDate))
          return;
        if (subscription.IsServiceHooksDelivery && stat.HitCount > jailSubscriptionServiceHooksThreshold || subscription.IsEmailDelivery && stat.HitCount > jailSubscriptionUserThreshold)
        {
          requestContext.Trace(1002915, TraceLevel.Error, "Notifications", nameof (NotificationSubscriptionService), "Jailing Subscription: {0}, Audit Only: {1}, subscription.ModifiedTime: {2}, hit count {3}, Channel {4}", (object) result, (object) auditOnly, (object) subscription.ModifiedTime, (object) stat.HitCount, (object) subscription.Channel);
          if (auditOnly)
            return;
          this.UpdateSubscriptionStatus(requestContext, subscription, SubscriptionStatus.JailedByNotificationsVolume, CoreRes.SubscriptionJailedByNotificationsVolume(), true);
        }
        else
          requestContext.Trace(1002913, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), "Skipping Jailing: {0} - subscription.ModifiedTime: {1} - jailOnlyfNewerThanDate = {2}", (object) result, (object) subscription.ModifiedTime, (object) jailOnlyfNewerThanDate);
      }
    }

    public NotificationSubscription GetNotificationSubscription(
      IVssRequestContext requestContext,
      string subscriptionId,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None)
    {
      Subscription subscription = this.GetSubscription(requestContext, subscriptionId, queryFlags);
      return MatcherFilterFactory.GetMatcherFilter(requestContext, subscription.Matcher).PrepareForDisplay(requestContext, subscription, queryFlags);
    }

    public List<NotificationSubscription> GetNotificationSubscriptions(
      IVssRequestContext requestContext,
      List<string> subscriptionIds,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None)
    {
      List<NotificationSubscription> notificationSubscriptionList = new List<NotificationSubscription>();
      List<Subscription> subscriptions = this.GetSubscriptions(requestContext, subscriptionIds, queryFlags);
      return this.ToNotificationSubscriptions(requestContext, (IEnumerable<Subscription>) subscriptions, queryFlags);
    }

    public Subscription GetSubscription(
      IVssRequestContext requestContext,
      string subscriptionId,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None)
    {
      IVssRequestContext requestContext1 = requestContext;
      List<string> subscriptionIds = new List<string>();
      subscriptionIds.Add(subscriptionId);
      int queryFlags1 = (int) queryFlags;
      return this.GetSubscriptions(requestContext1, subscriptionIds, (SubscriptionQueryFlags) queryFlags1).FirstOrDefault<Subscription>() ?? throw new SubscriptionNotFoundException();
    }

    public List<Subscription> GetSubscriptions(
      IVssRequestContext requestContext,
      List<string> subscriptionIds,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None)
    {
      List<Subscription> subscriptions = new List<Subscription>();
      List<SubscriptionLookup> subscriptionLookupList = new List<SubscriptionLookup>();
      List<string> contributedSubscriptionsLookup = new List<string>();
      foreach (string subscriptionId in subscriptionIds)
      {
        int result1;
        if (int.TryParse(subscriptionId, out result1))
        {
          subscriptionLookupList.Add(SubscriptionLookup.CreateSubscriptionIdLookup(result1));
        }
        else
        {
          Guid result2;
          if (Guid.TryParse(subscriptionId, out result2))
            subscriptionLookupList.Add(SubscriptionLookup.CreateUniqueIdLookup(result2));
          else
            contributedSubscriptionsLookup.Add(subscriptionId);
        }
      }
      if (subscriptionLookupList.Any<SubscriptionLookup>())
        subscriptions.AddRange((IEnumerable<Subscription>) this.QuerySubscriptions(requestContext, (IEnumerable<SubscriptionLookup>) subscriptionLookupList, queryFlags, true));
      if (contributedSubscriptionsLookup.Any<string>())
      {
        List<string> contributionTargets = new List<string>()
        {
          NotificationClientConstants.DefaultSubscriptionContributionTarget,
          NotificationClientConstants.DefaultSystemSubscriptionContributionTarget
        };
        IEnumerable<Subscription> contributedSubscriptions = this.GetContributedSubscriptions(requestContext, (IEnumerable<string>) contributionTargets, (IEnumerable<string>) null, true);
        subscriptions.AddRange(contributedSubscriptions.Where<Subscription>((Func<Subscription, bool>) (s => contributedSubscriptionsLookup.Contains(s.SubscriptionId))));
      }
      return subscriptions;
    }

    public Subscription GetSubscription(
      IVssRequestContext requestContext,
      int id,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None)
    {
      SubscriptionLookup subscriptionIdLookup = SubscriptionLookup.CreateSubscriptionIdLookup(id);
      requestContext.Trace(1002930, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), string.Format("QueryingSubscription Details of Subscription ID: {0}, Subscription Type: {1}", (object) 0, (object) 1), (object) subscriptionIdLookup.SubscriptionId, (object) subscriptionIdLookup.LookupType);
      return this.QuerySubscriptions(requestContext, subscriptionIdLookup, queryFlags, true).FirstOrDefault<Subscription>() ?? throw new SubscriptionNotFoundException();
    }

    private void PopulateUserSettings(
      IVssRequestContext requestContext,
      Subscription subscription,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = null)
    {
      this.PopulateUserSettings(requestContext, (IEnumerable<Subscription>) new Subscription[1]
      {
        subscription
      }, targetIdentity);
    }

    private void PopulateUserSettings(
      IVssRequestContext requestContext,
      IEnumerable<Subscription> subscriptions,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = null)
    {
      if (!subscriptions.Any<Subscription>())
        return;
      Guid guid = targetIdentity != null ? targetIdentity.Id : requestContext.GetUserId();
      if (guid.Equals(Guid.Empty))
        return;
      Dictionary<DefaultSubscriptionUserCandidate, Subscription> source = new Dictionary<DefaultSubscriptionUserCandidate, Subscription>();
      foreach (Subscription subscription in subscriptions)
      {
        if (subscription.IsOptOutable())
        {
          DefaultSubscriptionUserCandidate key = new DefaultSubscriptionUserCandidate()
          {
            SubscriberId = guid,
            SubscriptionName = subscription.SubscriptionId
          };
          source.Add(key, subscription);
        }
      }
      if (!source.Any<KeyValuePair<DefaultSubscriptionUserCandidate, Subscription>>())
        return;
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
      {
        List<DefaultSubscriptionUserCandidate> subscriptionsUserDisabled = component.GetDefaultSubscriptionsUserDisabled((IEnumerable<DefaultSubscriptionUserCandidate>) source.Keys);
        if (!subscriptionsUserDisabled.Any<DefaultSubscriptionUserCandidate>())
          return;
        HashSet<DefaultSubscriptionUserCandidate> subscriptionUserCandidateSet = new HashSet<DefaultSubscriptionUserCandidate>((IEnumerable<DefaultSubscriptionUserCandidate>) subscriptionsUserDisabled);
        foreach (KeyValuePair<DefaultSubscriptionUserCandidate, Subscription> keyValuePair in source)
        {
          if (subscriptionUserCandidateSet.Contains(keyValuePair.Key))
            keyValuePair.Value.UserSettingsOptedOut = true;
        }
      }
    }

    private void PopulateAdminSettings(
      IVssRequestContext requestContext,
      IEnumerable<Subscription> subscriptions)
    {
      if (!subscriptions.Any<Subscription>())
        return;
      HashSet<string> stringSet = new HashSet<string>();
      foreach (Subscription subscription in subscriptions)
      {
        if (subscription.IsOptOutable())
          stringSet.Add(subscription.SubscriptionId);
      }
      if (!stringSet.Any<string>())
        return;
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
      {
        List<DefaultSubscriptionAdminCandidate> subscriptionsAdminDisabled = component.GetDefaultSubscriptionsAdminDisabled(stringSet);
        Dictionary<string, DefaultSubscriptionAdminCandidate> adminSettingsMap = new Dictionary<string, DefaultSubscriptionAdminCandidate>();
        Action<DefaultSubscriptionAdminCandidate> action = (Action<DefaultSubscriptionAdminCandidate>) (item => adminSettingsMap.Add(item.SubscriptionName, item));
        subscriptionsAdminDisabled.ForEach(action);
        foreach (Subscription subscription in subscriptions)
        {
          DefaultSubscriptionAdminCandidate subscriptionAdminCandidate;
          if (adminSettingsMap.TryGetValue(subscription.SubscriptionId, out subscriptionAdminCandidate))
          {
            subscription.AdminSettingsBlockUserOptOut = subscriptionAdminCandidate.BlockUserDisable;
            if (subscription.IsContributed && subscriptionAdminCandidate.IsDisabled)
              subscription.Status = SubscriptionStatus.DisabledByAdmin;
          }
        }
      }
    }

    private void PopulateDiagnosticsForContributedSubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<Subscription> subscriptions)
    {
      if (!subscriptions.Any<Subscription>())
        return;
      RegistryEntryCollection source = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) (NotificationFrameworkConstants.ContributedSubscriptionDiagnosticsRoot + "/**"));
      if (!source.Any<RegistryEntry>())
        return;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (RegistryEntry registryEntry in source)
        dictionary[registryEntry.Name] = registryEntry.Value;
      foreach (Subscription subscription in subscriptions)
      {
        string json;
        if (dictionary.TryGetValue(subscription.SubscriptionId, out json))
          subscription.Diagnostics = DiagnosticUtils.DeserializeDiagnostics(requestContext, json);
      }
    }

    public NotificationSubscription UpdateSubscription(
      IVssRequestContext requestContext,
      string subscriptionId,
      NotificationSubscriptionUpdateParameters updateParameters,
      bool suspendPendingNotificationsOnDisable = true)
    {
      Subscription subscription = this.GetSubscription(requestContext, subscriptionId, SubscriptionQueryFlags.IncludeInvalidSubscriptions | SubscriptionQueryFlags.IncludeDeletedSubscriptions);
      if (subscription.IsContributed)
        this.ValidateContributedSubscriptionUpdateParameters(requestContext, updateParameters);
      SubscriptionUpdate subscriptionUpdate = (updateParameters.Filter == null ? MatcherFilterFactory.GetMatcherFilter(requestContext, subscription.Matcher) : MatcherFilterFactory.GetMatcherFilter(requestContext, updateParameters.Filter)).PrepareForUpdate(requestContext, subscription, updateParameters);
      this.UpdateSubscriptionInternal(requestContext, subscription, subscriptionUpdate, suspendPendingNotificationsOnDisable);
      return this.GetNotificationSubscription(requestContext, subscriptionId, SubscriptionQueryFlags.IncludeFilterDetails);
    }

    private void ValidateContributedSubscriptionUpdateParameters(
      IVssRequestContext requestContext,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      ArgumentUtility.CheckForNull<NotificationSubscriptionUpdateParameters>(updateParameters, "UpdateParameters");
      if (updateParameters.Channel != null || updateParameters.Description != null || updateParameters.Filter != null || updateParameters.Scope != null)
        throw new ArgumentException(CoreRes.AllowedFieldUpdatesForContributedSubscription());
      if (!updateParameters.Status.HasValue && updateParameters.UserSettings == null && updateParameters.AdminSettings == null)
        throw new ArgumentException(CoreRes.AllowedFieldUpdatesForContributedSubscription());
      if (updateParameters.Status.HasValue && updateParameters.Status.Value != SubscriptionStatus.Disabled && updateParameters.Status.Value != SubscriptionStatus.DisabledByAdmin && updateParameters.Status.Value != SubscriptionStatus.Enabled)
        throw new ArgumentException(CoreRes.InvalidValueException((object) updateParameters.Status.Value, (object) "Status"));
    }

    public void UpdateSubscriptionStatus(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionStatus newStatus,
      string newStatusMessage = null,
      bool suspendPendingNotificationsOnDisable = true)
    {
      if (newStatus != SubscriptionStatus.Enabled && string.IsNullOrEmpty(newStatusMessage))
        newStatusMessage = newStatus != SubscriptionStatus.JailedByNotificationsVolume ? CoreRes.SubscriptionStatusDefaultMessage((object) newStatus) : CoreRes.SubscriptionJailedByNotificationsVolume();
      SubscriptionUpdate subscriptionUpdate = new SubscriptionUpdate(subscription.ID)
      {
        Status = new SubscriptionStatus?(newStatus),
        StatusMessage = newStatusMessage
      };
      this.UpdateSubscriptionInternal(requestContext, subscription, subscriptionUpdate, suspendPendingNotificationsOnDisable);
    }

    public void UpdateSubscriptionDiagnostics(
      IVssRequestContext requestContext,
      IEnumerable<Subscription> subscriptions)
    {
      List<SubscriptionUpdate> subscriptionUpdateList = new List<SubscriptionUpdate>();
      List<SubscriptionUpdate> source = new List<SubscriptionUpdate>();
      foreach (Subscription subscription in subscriptions)
      {
        SubscriptionUpdate subscriptionUpdate = new SubscriptionUpdate(subscription.SubscriptionId)
        {
          Diagnostics = subscription.Diagnostics
        };
        if (!subscription.IsContributed)
          subscriptionUpdateList.Add(subscriptionUpdate);
        else
          source.Add(subscriptionUpdate);
      }
      if (subscriptionUpdateList.Any<SubscriptionUpdate>())
      {
        using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
          component.UpdateEventSubscription(new List<SubscriptionUpdate>((IEnumerable<SubscriptionUpdate>) subscriptionUpdateList));
      }
      if (!source.Any<SubscriptionUpdate>())
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      foreach (SubscriptionUpdate subscriptionUpdate in source)
      {
        string path = NotificationFrameworkConstants.ContributedSubscriptionDiagnosticsRoot + "/" + subscriptionUpdate.ContributionId;
        service.SetValue<string>(requestContext, path, DiagnosticUtils.SerializeDiagnostics(subscriptionUpdate.Diagnostics));
      }
    }

    public void UpdateSubscription(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      SubscriptionUpdate subscriptionUpdate,
      bool suspendPendingNotificationsOnDisable = true)
    {
      if (!originalSubscription.IsContributed && !subscriptionUpdate.IsNoCoreFieldsUpdate())
      {
        string str = NotifHelpers.FirstNonEmptyString(subscriptionUpdate.Matcher, originalSubscription.Matcher);
        string eventType = NotifHelpers.FirstNonEmptyString(subscriptionUpdate.EventTypeName, originalSubscription.EventTypeName);
        this.VerifyMatcher(requestContext, str);
        this.VerifyEventType(requestContext, eventType);
        this.VerifyMatcherCompatibleWithEvent(requestContext, str, eventType);
        MatcherFilterFactory.GetMatcherFilter(requestContext, str).PrepareForUpdate(requestContext, originalSubscription, subscriptionUpdate);
      }
      this.UpdateSubscriptionInternal(requestContext, originalSubscription, subscriptionUpdate, suspendPendingNotificationsOnDisable);
    }

    private void UpdateSubscriptionInternal(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      SubscriptionUpdate subscriptionUpdate,
      bool suspendPendingNotificationsOnDisable)
    {
      IEventNotificationServiceInternal service = requestContext.GetService<IEventNotificationServiceInternal>();
      if (!originalSubscription.IsContributed && !subscriptionUpdate.IsNoCoreFieldsUpdate())
      {
        if (!this.CreateOrUpdateSubscriptionAllowed(requestContext, originalSubscription.SubscriberIdentity, originalSubscription.Matcher))
          throw new UnauthorizedAccessException(CoreRes.UnauthorizedUpdateSubscription());
        using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
          component.UpdateEventSubscription(new List<SubscriptionUpdate>()
          {
            subscriptionUpdate
          });
        NotificationAuditing.PublishUpdateSubscriptionEvent(requestContext, originalSubscription, subscriptionUpdate);
      }
      if (subscriptionUpdate.Status.HasValue && subscriptionUpdate.Status.Value < SubscriptionStatus.Enabled)
      {
        foreach (INotificationsSystemEventFactory eventFactoryImpl in (IEnumerable<INotificationsSystemEventFactory>) this.m_eventFactoryImpls)
        {
          VssNotificationEvent subscriptionDisabledEvent = eventFactoryImpl.CreateSubscriptionDisabledEvent(requestContext, originalSubscription, subscriptionUpdate);
          if (subscriptionDisabledEvent != null)
            requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, subscriptionDisabledEvent);
        }
      }
      if (originalSubscription.IsGroup && (subscriptionUpdate.AdminSettings != null || subscriptionUpdate.Status.HasValue))
      {
        if (!NotificationSubscriptionSecurityUtils.HasPermissions(requestContext, originalSubscription.SubscriberIdentity, 2))
          throw new UnauthorizedAccessException(CoreRes.UnauthorizedChangeStatusSubscription());
        bool disabled = ((int) subscriptionUpdate.Status ?? (int) originalSubscription.Status) < 0;
        bool blockUserDisable = subscriptionUpdate.AdminSettings != null ? subscriptionUpdate.AdminSettings.BlockUserOptOut : originalSubscription.AdminSettingsBlockUserOptOut;
        Guid userId = requestContext.GetUserId();
        using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
          component.UpdateDefaultSubscriptionsAdminEnabled(originalSubscription.SubscriptionId, disabled, blockUserDisable, userId);
        NotificationAuditing.PublishUpdateSubscriptionAdminSettingsEvent(requestContext, originalSubscription, subscriptionUpdate.Status, subscriptionUpdate.AdminSettings?.BlockUserOptOut);
      }
      if (!subscriptionUpdate.Status.HasValue || subscriptionUpdate.Status.Value >= SubscriptionStatus.Enabled || !suspendPendingNotificationsOnDisable)
        return;
      service.SuspendUnprocessedNotifications(requestContext, originalSubscription);
    }

    public void UpdateSubscriptionStatus(
      IVssRequestContext requestContext,
      int subscriptionId,
      SubscriptionStatus newStatus,
      string newStatusMessage = null,
      bool suspendPendingNotificationsOnDisable = true)
    {
      Subscription subscription = this.GetSubscription(requestContext, subscriptionId, SubscriptionQueryFlags.IncludeDeletedSubscriptions);
      this.UpdateSubscriptionStatus(requestContext, subscription, newStatus, newStatusMessage, suspendPendingNotificationsOnDisable);
    }

    public List<NotificationSubscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      SubscriptionQuery subscriptionQuery)
    {
      IEnumerable<Subscription> subscriptions = this.QuerySubscriptionsInternal(requestContext, subscriptionQuery);
      SubscriptionQueryFlags queryFlags = subscriptionQuery.QueryFlags.HasValue ? subscriptionQuery.QueryFlags.Value : SubscriptionQueryFlags.None;
      using (PerformanceTimer.StartMeasure(requestContext, "QuerySubscriptions_ToNotificationSubscriptions"))
        return this.ToNotificationSubscriptions(requestContext, subscriptions, queryFlags);
    }

    private List<NotificationSubscription> ToNotificationSubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<Subscription> subscriptions,
      SubscriptionQueryFlags queryFlags)
    {
      List<NotificationSubscription> notificationSubscriptions = new List<NotificationSubscription>();
      if (subscriptions == null)
        return notificationSubscriptions;
      foreach (Subscription subscription in subscriptions)
      {
        try
        {
          IMatcherFilter matcherFilter = MatcherFilterFactory.GetMatcherFilter(requestContext, subscription.Matcher);
          notificationSubscriptions.Add(matcherFilter.PrepareForDisplay(requestContext, subscription, queryFlags));
        }
        catch (Exception ex)
        {
          requestContext.Trace(1002020, TraceLevel.Warning, "Notifications", nameof (NotificationSubscriptionService), subscription.TraceTags, "Unable to post process a Subscription returned in a query. Will not return with result set. Exception: {0}", (object) ex);
        }
      }
      return notificationSubscriptions;
    }

    public List<Subscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      SubscriptionLookup subscriptionKey,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None,
      bool updateProjectGuid = true)
    {
      IVssRequestContext requestContext1 = requestContext;
      List<SubscriptionLookup> subscriptionKeys = new List<SubscriptionLookup>();
      subscriptionKeys.Add(subscriptionKey);
      int queryFlags1 = (int) queryFlags;
      int num = updateProjectGuid ? 1 : 0;
      return this.QueryCustomSubscriptions(requestContext1, (IEnumerable<SubscriptionLookup>) subscriptionKeys, (SubscriptionQueryFlags) queryFlags1, num != 0);
    }

    public List<Subscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<SubscriptionLookup> subscriptionKeys,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None,
      bool updateProjectGuid = true)
    {
      return this.QueryCustomSubscriptions(requestContext, subscriptionKeys, queryFlags, updateProjectGuid);
    }

    private IEnumerable<Subscription> QuerySubscriptionsInternal(
      IVssRequestContext requestContext,
      SubscriptionQuery subscriptionQuery,
      bool updateProjectGuid = true)
    {
      this.ValidateNotificationSubscriptionQuery(requestContext, subscriptionQuery);
      List<Subscription> subscriptionList = new List<Subscription>();
      List<SubscriptionLookup> subscriptionLookupList1 = new List<SubscriptionLookup>();
      List<SubscriptionLookup> subscriptionLookupList2 = new List<SubscriptionLookup>();
      HashSet<string> limitToContributionNames = new HashSet<string>();
      List<ISubscriptionFilter> filtersForContributed = new List<ISubscriptionFilter>();
      HashSet<string> contributionTargets = new HashSet<string>();
      this.GetValidUsersGroup(requestContext);
      SubscriptionQueryFlags queryFlags = subscriptionQuery.QueryFlags.HasValue ? subscriptionQuery.QueryFlags.Value : SubscriptionQueryFlags.None;
      bool flag1 = false;
      foreach (SubscriptionQueryCondition condition in subscriptionQuery.Conditions)
      {
        SubscriptionLookup query = this.ParseQuery(requestContext, condition);
        bool flag2 = false;
        SubscriptionFlags? flags = query.Flags;
        int num;
        if (flags.HasValue)
        {
          flags = query.Flags;
          if (flags.Value.HasFlag((Enum) SubscriptionFlags.ContributedSubscription))
          {
            num = !string.IsNullOrEmpty(query.ContributedSubscriptionName) ? 1 : 0;
            goto label_6;
          }
        }
        num = 0;
label_6:
        if (num != 0)
        {
          limitToContributionNames.Add(query.ContributedSubscriptionName);
          flag2 = true;
        }
        else
        {
          flags = query.Flags;
          if (flags.HasValue)
          {
            flags = query.Flags;
            if (!flags.Value.HasFlag((Enum) SubscriptionFlags.GroupSubscription))
            {
              flags = query.Flags;
              if (!flags.Value.HasFlag((Enum) SubscriptionFlags.ContributedSubscription))
                goto label_12;
            }
            flag2 = true;
          }
label_12:
          subscriptionLookupList1.Add(query);
        }
        if (flag2)
        {
          flag1 = true;
          if (condition.Filter != null)
            filtersForContributed.Add(condition.Filter);
          else
            filtersForContributed.Add((ISubscriptionFilter) new ActorFilter((string) null, (ExpressionFilterModel) null));
          contributionTargets.Add(NotificationClientConstants.DefaultSubscriptionContributionTarget);
          if (queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeSystemSubscriptions))
            contributionTargets.Add(NotificationClientConstants.DefaultSystemSubscriptionContributionTarget);
        }
      }
      if (subscriptionLookupList1.Any<SubscriptionLookup>())
      {
        using (PerformanceTimer.StartMeasure(requestContext, "QuerySubscriptionsInternal_QueryCustomSubscriptions"))
          subscriptionList.AddRange((IEnumerable<Subscription>) this.QueryCustomSubscriptions(requestContext, (IEnumerable<SubscriptionLookup>) subscriptionLookupList1, queryFlags, updateProjectGuid));
      }
      if (flag1)
      {
        using (PerformanceTimer.StartMeasure(requestContext, "QuerySubscriptionsInternal_QueryContributedSubscritptions"))
          subscriptionList.AddRange(this.QueryContributedSubscriptions(requestContext, (IEnumerable<string>) contributionTargets, filtersForContributed, limitToContributionNames));
      }
      return (IEnumerable<Subscription>) subscriptionList;
    }

    private List<Subscription> QueryCustomSubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<SubscriptionLookup> subscriptionKeys,
      SubscriptionQueryFlags queryFlags,
      bool updateProjectGuid = true,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = null)
    {
      bool includeDeleted = queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeDeletedSubscriptions);
      List<Subscription> subscriptions;
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
        subscriptions = component.QuerySubscriptions(subscriptionKeys, includeDeleted);
      HashSet<Guid> expandSubscriberIds = new HashSet<Guid>();
      HashSet<int> expandSubscriptionIds = new HashSet<int>();
      foreach (SubscriptionLookup subscriptionKey in subscriptionKeys)
      {
        Guid? subscriberId = subscriptionKey.SubscriberId;
        if (subscriberId.HasValue)
        {
          subscriberId = subscriptionKey.SubscriberId;
          Guid empty = Guid.Empty;
          if ((subscriberId.HasValue ? (subscriberId.HasValue ? (subscriberId.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          {
            HashSet<Guid> guidSet = expandSubscriberIds;
            subscriberId = subscriptionKey.SubscriberId;
            Guid guid = subscriberId.Value;
            guidSet.Add(guid);
            continue;
          }
        }
        int? subscriptionId = subscriptionKey.SubscriptionId;
        if (subscriptionId.HasValue)
        {
          subscriptionId = subscriptionKey.SubscriptionId;
          int num1 = 0;
          if (subscriptionId.GetValueOrDefault() > num1 & subscriptionId.HasValue)
          {
            HashSet<int> intSet = expandSubscriptionIds;
            subscriptionId = subscriptionKey.SubscriptionId;
            int num2 = subscriptionId.Value;
            intSet.Add(num2);
          }
        }
      }
      List<Subscription> subscriptionList = this.FilterOutSubscriptions(requestContext, subscriptions, queryFlags, updateProjectGuid, expandSubscriberIds, expandSubscriptionIds, targetIdentity);
      using (PerformanceTimer.StartMeasure(requestContext, "QueryCustomSubscriptions_PopulateAdminSettings"))
        this.PopulateAdminSettings(requestContext, (IEnumerable<Subscription>) subscriptionList);
      using (PerformanceTimer.StartMeasure(requestContext, "QueryCustomSubscriptions_PopulateUserSettings"))
        this.PopulateUserSettings(requestContext, (IEnumerable<Subscription>) subscriptionList, targetIdentity);
      return subscriptionList;
    }

    private void ValidateNotificationSubscriptionQuery(
      IVssRequestContext requestContext,
      SubscriptionQuery subscriptionQuery)
    {
      ArgumentUtility.CheckForNull<SubscriptionQuery>(subscriptionQuery, nameof (subscriptionQuery));
      ArgumentUtility.CheckForNull<IEnumerable<SubscriptionQueryCondition>>(subscriptionQuery.Conditions, "subscriptionQuery.Conditions");
      List<SubscriptionQueryCondition> source = new List<SubscriptionQueryCondition>();
      bool flag = false;
      foreach (SubscriptionQueryCondition condition in subscriptionQuery.Conditions)
      {
        if (!string.IsNullOrEmpty(condition.SubscriptionId) || condition.SubscriberId.HasValue || !string.IsNullOrEmpty(condition.Scope) || condition.Filter != null && !(condition.Filter is UnsupportedFilter))
        {
          source.Add(condition);
        }
        else
        {
          SubscriptionFlags? flags = condition.Flags;
          if (flags.HasValue)
          {
            flags = condition.Flags;
            if (flags.Value.HasFlag((Enum) SubscriptionFlags.ContributedSubscription))
              flag = true;
          }
        }
      }
      if (!source.Any<SubscriptionQueryCondition>() && !flag)
        throw new InvalidNotificationSubscriptionQueryException(CoreRes.MustProvideAtLeastOneCondition());
    }

    private List<Subscription> FilterOutSubscriptions(
      IVssRequestContext requestContext,
      List<Subscription> subscriptions,
      SubscriptionQueryFlags queryFlags,
      bool updateProjectGuid,
      HashSet<Guid> expandSubscriberIds = null,
      HashSet<int> expandSubscriptionIds = null,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = null)
    {
      return requestContext.IsFeatureEnabled("Notifications.EnableNewFilterOut") ? NotificationSubscriptionService.FilterOutSubscriptionsNew(requestContext, subscriptions, queryFlags, updateProjectGuid, expandSubscriberIds, expandSubscriptionIds, targetIdentity) : NotificationSubscriptionService.FilterOutSubscriptionsOld(requestContext, subscriptions, queryFlags, updateProjectGuid, expandSubscriberIds, expandSubscriptionIds, targetIdentity: targetIdentity);
    }

    private static List<Subscription> FilterOutSubscriptionsOld(
      IVssRequestContext requestContext,
      List<Subscription> subscriptions,
      SubscriptionQueryFlags queryFlags,
      bool updateProjectGuid,
      HashSet<Guid> expandSubscriberIds = null,
      HashSet<int> expandSubscriptionIds = null,
      IdentityDescriptor memberDescriptor = null,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = null)
    {
      if (!subscriptions.Any<Subscription>())
        return subscriptions;
      IdentityService service = requestContext.GetService<IdentityService>();
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      Dictionary<string, NotificationEventType> supportedEventTypes = requestContext.GetService<INotificationEventService>().GetKeyedEventTypes(requestContext);
      bool includeInvalidSubscriptions = queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeInvalidSubscriptions);
      subscriptions = subscriptions.Where<Subscription>((Func<Subscription, bool>) (x => includeInvalidSubscriptions || x.HasValidEventType(requestContext, supportedEventTypes))).OrderBy<Subscription, Guid>((Func<Subscription, Guid>) (s =>
      {
        identityMap[s.SubscriberId] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        return s.SubscriberId;
      })).ToList<Subscription>();
      bool flag1 = false;
      if (targetIdentity != null)
        flag1 = true;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(requestContext, (IList<Guid>) identityMap.Keys.ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null))
      {
        if (readIdentity != null)
          identityMap[readIdentity.Id] = readIdentity;
      }
      List<Subscription> subscriptionList = new List<Subscription>();
      Guid g1 = Guid.Empty;
      Guid g2 = Guid.Empty;
      bool flag2 = true;
      bool flag3 = false;
      using (PerformanceTimer.StartMeasure(requestContext, "CallerHasAdminPermissions"))
        flag3 = NotificationSubscriptionSecurityUtils.CallerHasAdminPermissions(requestContext, 2);
      bool flag4 = false;
      bool flag5 = false;
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      foreach (Subscription subscription1 in subscriptions)
      {
        bool flag6 = subscription1.SubscriberId.Equals(g1);
        if (!(!flag2 & flag6) || queryFlags.HasFlag((Enum) SubscriptionQueryFlags.AlwaysReturnBasicInformation))
        {
          if (!flag6)
          {
            identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
            bool flag7 = expandSubscriberIds != null && expandSubscriberIds.Contains(subscription1.SubscriberId) || expandSubscriptionIds != null && expandSubscriptionIds.Contains(subscription1.ID);
            bool isGroupAdmin = false;
            bool hasDoneGroupAdminCheck = false;
            flag4 = false;
            g1 = subscription1.SubscriberId;
            if (identityMap.TryGetValue(subscription1.SubscriberId, out identity) && identity != null)
            {
              g2 = Guid.Empty;
              bool flag8 = false;
              using (PerformanceTimer.StartMeasure(requestContext, "FilterOutSubscriptions_CallerHasReadPermission"))
                flag8 = flag3 || (flag7 ? NotificationSubscriptionSecurityUtils.HasPermissions(requestContext, identity, 1, out isGroupAdmin, out hasDoneGroupAdminCheck) : NotificationSubscriptionSecurityUtils.HasPermissionsNoGroupAdminCheck(requestContext, identity, 1));
              flag2 = flag8;
              if (!requestContext.IsSystemContext && identity.IsContainer && !flag7)
              {
                IdentityDescriptor x = memberDescriptor != (IdentityDescriptor) null ? memberDescriptor : requestContext.GetUserIdentity().Descriptor;
                using (PerformanceTimer.StartMeasure(requestContext, "FilterOutSubscriptions_CallerIsMember"))
                  flag2 = flag2 && (!flag1 || IdentityDescriptorComparer.Instance.Equals(x, identity.Descriptor) || service.IsMember(requestContext, identity.Descriptor, targetIdentity.Descriptor));
              }
              using (PerformanceTimer.StartMeasure(requestContext, "FilterOutSubscriptions_CallerHasWritePermission"))
              {
                if (flag2)
                  flag4 = flag3 | isGroupAdmin || (flag7 ? !hasDoneGroupAdminCheck && NotificationSubscriptionSecurityUtils.HasPermissions(requestContext, identity, 2) : NotificationSubscriptionSecurityUtils.HasPermissionsNoGroupAdminCheck(requestContext, identity, 2));
              }
              if (flag2 && identity.IsContainer)
                flag5 = subscription1.IsTeam || identity.IsTeam(requestContext);
            }
            else
            {
              g2 = subscription1.SubscriberId;
              flag2 = flag3 && includeInvalidSubscriptions;
            }
          }
          if (!flag1 || flag2 && !subscription1.SubscriberId.Equals(g2))
          {
            Subscription subscription2;
            if (!flag2)
            {
              if (queryFlags.HasFlag((Enum) SubscriptionQueryFlags.AlwaysReturnBasicInformation))
              {
                subscription1.Permissions = SubscriptionPermissions.None;
                subscription2 = subscription1.CloneBasic();
              }
              else
              {
                requestContext.Trace(1002112, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), subscription1.TraceTags, "Filtering out result - user {0} does not have permission to view subscription {1} (subscriber id = {2})", (object) requestContext.GetUserIdentity().Id, (object) subscription1.ID, (object) subscription1.SubscriberId);
                continue;
              }
            }
            else
            {
              subscription1.Permissions = SubscriptionPermissions.View;
              if (flag4)
                subscription1.Permissions = subscription1.Permissions | SubscriptionPermissions.Edit | SubscriptionPermissions.Delete;
              if (flag5)
                subscription1.Flags |= SubscriptionFlags.TeamSubscription;
              subscription2 = subscription1;
            }
            if (!subscription2.SubscriberId.Equals(g2))
            {
              try
              {
                using (PerformanceTimer.StartMeasure(requestContext, "FilterOutSubscriptions_PostBindSubscription"))
                  subscription2.PostBindSubscription(requestContext, queryFlags);
              }
              catch (IdentityNotFoundException ex)
              {
                requestContext.TraceException(1002114, "Notifications", nameof (NotificationSubscriptionService), (Exception) ex);
                g2 = subscription2.SubscriberId;
                if (!flag3)
                {
                  flag2 = false;
                  continue;
                }
              }
              catch (Exception ex)
              {
                if (!queryFlags.HasFlag((Enum) SubscriptionQueryFlags.AlwaysReturnBasicInformation))
                {
                  requestContext.TraceException(1002114, "Notifications", nameof (NotificationSubscriptionService), ex);
                  throw;
                }
              }
              if (flag1 && identity.IsContainer && targetIdentity.IsContainer && identity.Id != targetIdentity.Id)
              {
                string contributed = EventTypeMapper.ToContributed(requestContext, subscription1.EventTypeName);
                NotificationEventType eventType = supportedEventTypes[contributed];
                if (!NotificationSubscriptionService.IsGroupOptOutable(requestContext, subscription1, identity, eventType))
                  continue;
              }
            }
            subscriptionList.Add(subscription2);
          }
        }
      }
      return subscriptionList;
    }

    private static List<Subscription> FilterOutSubscriptionsNew(
      IVssRequestContext requestContext,
      List<Subscription> subscriptions,
      SubscriptionQueryFlags queryFlags,
      bool updateProjectGuid,
      HashSet<Guid> expandSubscriberIds,
      HashSet<int> expandSubscriptionIds,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = null)
    {
      if (!subscriptions.Any<Subscription>())
        return subscriptions;
      List<Subscription> subscriptionList = new List<Subscription>();
      using (PerformanceTimer.StartMeasure(requestContext, "FilterOutSubscriptions", string.Format("Count={0}", (object) subscriptions.Count)))
      {
        bool callerHasAdminPermissions;
        using (PerformanceTimer.StartMeasure(requestContext, "CallerHasAdminPermissions"))
          callerHasAdminPermissions = NotificationSubscriptionSecurityUtils.CallerHasAdminPermissions(requestContext, 2);
        bool flag1 = targetIdentity != null;
        bool includeInvalidSubscriptions = queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeInvalidSubscriptions);
        HashSet<Guid> subscriberIds = new HashSet<Guid>();
        HashSet<Guid> expandIdsToCheckForGroupPermission = new HashSet<Guid>();
        Dictionary<string, NotificationEventType> supportedEventTypes;
        using (PerformanceTimer.StartMeasure(requestContext, "SortAndValidateEventTypes"))
        {
          supportedEventTypes = requestContext.GetService<INotificationEventService>().GetKeyedEventTypes(requestContext);
          subscriptions = subscriptions.Where<Subscription>((Func<Subscription, bool>) (x => includeInvalidSubscriptions || x.HasValidEventType(requestContext, supportedEventTypes))).OrderBy<Subscription, Guid>((Func<Subscription, Guid>) (s =>
          {
            if (!callerHasAdminPermissions && s.IsGroup && (expandSubscriptionIds != null && expandSubscriptionIds.Contains(s.ID) || expandSubscriberIds != null && expandSubscriberIds.Contains(s.SubscriberId)))
              expandIdsToCheckForGroupPermission.Add(s.SubscriberId);
            subscriberIds.Add(s.SubscriberId);
            return s.SubscriberId;
          })).ToList<Subscription>();
        }
        Dictionary<Guid, NotificationSubscriptionService.IdentityPermissions> dictionary = NotificationSubscriptionService.ProcessIdentitiesForFiltering(requestContext, subscriberIds, expandIdsToCheckForGroupPermission, targetIdentity, callerHasAdminPermissions);
        Guid empty = Guid.Empty;
        Guid g = Guid.Empty;
        bool flag2 = true;
        bool flag3 = false;
        Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        SubscriptionPermissions subscriptionPermissions = SubscriptionPermissions.None;
        using (PerformanceTimer.StartMeasure(requestContext, "FilterElligibleSubscriptions", string.Format("Count={0}", (object) subscriptions.Count)))
        {
          foreach (Subscription subscription1 in subscriptions)
          {
            bool flag4 = subscription1.SubscriberId.Equals(empty);
            if (!(!flag2 & flag4) || queryFlags.HasFlag((Enum) SubscriptionQueryFlags.AlwaysReturnBasicInformation))
            {
              if (!flag4)
              {
                identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
                subscriptionPermissions = SubscriptionPermissions.None;
                int num = expandSubscriberIds == null ? 0 : (expandSubscriberIds.Contains(subscription1.SubscriberId) ? 1 : 0);
                bool flag5 = expandSubscriptionIds != null && expandSubscriptionIds.Contains(subscription1.ID);
                bool flag6 = num == 0 && !flag5;
                NotificationSubscriptionService.IdentityPermissions identityPermissions;
                if (dictionary.TryGetValue(subscription1.SubscriberId, out identityPermissions))
                {
                  g = Guid.Empty;
                  identity = identityPermissions.Identity;
                  subscriptionPermissions = identityPermissions.Permissions;
                  flag2 = subscriptionPermissions != 0;
                  if (((!flag2 ? 0 : (identity.IsContainer ? 1 : 0)) & (flag1 ? 1 : 0) & (flag6 ? 1 : 0)) != 0)
                    flag2 = identityPermissions.IsMember || IdentityDescriptorComparer.Instance.Equals(requestContext.GetUserIdentity().Descriptor, identity.Descriptor);
                  if (flag2 && identity.IsContainer)
                    flag3 = subscription1.IsTeam || identity.IsTeam(requestContext);
                }
                else
                {
                  g = subscription1.SubscriberId;
                  flag2 = callerHasAdminPermissions && includeInvalidSubscriptions;
                }
              }
              if (!flag1 || flag2 && !subscription1.SubscriberId.Equals(g))
              {
                Subscription subscription2;
                if (!flag2)
                {
                  if (queryFlags.HasFlag((Enum) SubscriptionQueryFlags.AlwaysReturnBasicInformation))
                  {
                    subscription1.Permissions = SubscriptionPermissions.None;
                    subscription2 = subscription1.CloneBasic();
                  }
                  else
                  {
                    requestContext.Trace(1002112, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), subscription1.TraceTags, "Filtering out result - user {0} does not have permission to view subscription {1} (subscriber id = {2})", (object) requestContext.GetUserIdentity().Id, (object) subscription1.ID, (object) subscription1.SubscriberId);
                    continue;
                  }
                }
                else
                {
                  subscription1.Permissions = subscriptionPermissions;
                  if (flag3)
                    subscription1.Flags |= SubscriptionFlags.TeamSubscription;
                  subscription2 = subscription1;
                }
                if (!subscription2.SubscriberId.Equals(g))
                {
                  try
                  {
                    using (PerformanceTimer.StartMeasure(requestContext, "PostBind", string.Format("Id={0}", (object) subscription2.ID)))
                      subscription2.PostBindSubscription(requestContext, queryFlags);
                  }
                  catch (IdentityNotFoundException ex)
                  {
                    requestContext.TraceException(1002115, "Notifications", nameof (NotificationSubscriptionService), (Exception) ex);
                    g = subscription2.SubscriberId;
                    if (!callerHasAdminPermissions)
                    {
                      flag2 = false;
                      continue;
                    }
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(1002115, "Notifications", nameof (NotificationSubscriptionService), ex);
                    if (!queryFlags.HasFlag((Enum) SubscriptionQueryFlags.AlwaysReturnBasicInformation))
                    {
                      if (!queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeInvalidSubscriptions))
                        throw;
                    }
                  }
                  if (flag1 && identity.IsContainer && targetIdentity.IsContainer && identity.Id != targetIdentity.Id)
                  {
                    string contributed = EventTypeMapper.ToContributed(requestContext, subscription1.EventTypeName);
                    NotificationEventType eventType;
                    if (supportedEventTypes.TryGetValue(contributed, out eventType))
                    {
                      if (!NotificationSubscriptionService.IsGroupOptOutable(requestContext, subscription1, identity, eventType))
                        continue;
                    }
                    else if (!includeInvalidSubscriptions)
                      continue;
                  }
                }
                subscriptionList.Add(subscription2);
              }
            }
          }
        }
      }
      requestContext.TracePerformanceTimers(1002112, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService));
      return subscriptionList;
    }

    private static Dictionary<Guid, NotificationSubscriptionService.IdentityPermissions> ProcessIdentitiesForFiltering(
      IVssRequestContext requestContext,
      HashSet<Guid> subscriberIds,
      HashSet<Guid> expandIdsToCheckForGroupPermission,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      bool callerHasAdminPermissions)
    {
      using (PerformanceTimer.StartMeasure(requestContext, nameof (ProcessIdentitiesForFiltering), string.Format("Count={0}", (object) subscriberIds.Count)))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        Dictionary<Guid, NotificationSubscriptionService.IdentityPermissions> dictionary = new Dictionary<Guid, NotificationSubscriptionService.IdentityPermissions>();
        List<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        bool flag1 = targetIdentity != null;
        List<Guid> list = subscriberIds.ToList<Guid>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2;
        using (PerformanceTimer.StartMeasure(requestContext, "ReadIdentities"))
          identityList2 = service.ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
        for (int index = 0; index < identityList2.Count; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity subscriptionIdentity = identityList2[index];
          if (subscriptionIdentity != null)
          {
            NotificationSubscriptionService.IdentityPermissions identityPermissions = new NotificationSubscriptionService.IdentityPermissions();
            identityPermissions.Identity = subscriptionIdentity;
            bool flag2 = false;
            if ((callerHasAdminPermissions ? 1 : (NotificationSubscriptionSecurityUtils.HasPermissionsNoGroupAdminCheck(requestContext, subscriptionIdentity, 1) ? 1 : 0)) != 0)
            {
              bool flag3 = callerHasAdminPermissions || NotificationSubscriptionSecurityUtils.HasPermissionsNoGroupAdminCheck(requestContext, subscriptionIdentity, 2);
              identityPermissions.Permissions |= flag3 ? SubscriptionPermissions.View | SubscriptionPermissions.Edit | SubscriptionPermissions.Delete : SubscriptionPermissions.View;
              requestContext.Trace(1002112, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), "Identity {0} has {1} permissions via cheap calls", (object) subscriptionIdentity.DisplayName, (object) (int) identityPermissions.Permissions);
              if (subscriptionIdentity.IsContainer)
              {
                if (flag1)
                {
                  bool flag4;
                  using (PerformanceTimer.StartMeasure(requestContext, "IsMember", string.Format("Id={0}", (object) subscriptionIdentity.Id)))
                    flag4 = service.IsMember(requestContext, subscriptionIdentity.Descriptor, targetIdentity.Descriptor);
                  identityPermissions.IsMember = flag4;
                  flag2 = identityPermissions.IsMember && !flag3;
                }
                else
                  flag2 = !flag3;
              }
            }
            if (flag2 || expandIdsToCheckForGroupPermission.Contains(subscriptionIdentity.Id) || requestContext.IsSystemContext)
            {
              identityList1.Add(subscriptionIdentity);
              requestContext.Trace(1002112, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), "Identity {0} needs group admin check", (object) subscriptionIdentity.DisplayName);
            }
            dictionary[subscriptionIdentity.Id] = identityPermissions;
          }
          else
            requestContext.Trace(1002112, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), "VSID {0} not found", (object) list[index]);
        }
        if (identityList1.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        {
          List<bool> adminPermissions;
          using (PerformanceTimer.StartMeasure(requestContext, "GetBulkAdminPermissions", string.Format("Count={0}", (object) identityList1.Count)))
            adminPermissions = NotificationSubscriptionSecurityUtils.GetBulkGroupAdminPermissions(requestContext, identityList1);
          for (int index = 0; index < adminPermissions.Count; ++index)
          {
            if (adminPermissions[index])
            {
              Guid id = identityList1[index].Id;
              NotificationSubscriptionService.IdentityPermissions identityPermissions;
              if (dictionary.TryGetValue(id, out identityPermissions))
              {
                identityPermissions.Permissions = SubscriptionPermissions.View | SubscriptionPermissions.Edit | SubscriptionPermissions.Delete;
                requestContext.Trace(1002112, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), "Identity {0} has group admin permission", (object) identityPermissions.Identity.DisplayName);
              }
              else
                requestContext.Trace(1002113, TraceLevel.Error, "Notifications", nameof (NotificationSubscriptionService), "Could not find {0} in identityMap - most definitely unexpected", (object) id);
            }
          }
        }
        return dictionary;
      }
    }

    private IEnumerable<Subscription> FilterContributedSubscriptionsForQuery(
      IVssRequestContext requestContext,
      IEnumerable<Subscription> contributedSubscriptions,
      List<ISubscriptionFilter> filtersForContributed,
      HashSet<string> limitToContributionNames = null,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = null)
    {
      List<Subscription> source = new List<Subscription>();
      if (filtersForContributed == null || filtersForContributed.Count<ISubscriptionFilter>() == 0)
      {
        source = contributedSubscriptions.ToList<Subscription>();
      }
      else
      {
        List<Tuple<string, string>> tupleList = new List<Tuple<string, string>>();
        foreach (ISubscriptionFilter subscriptionFilter in filtersForContributed)
        {
          string str;
          if (NotificationSubscriptionService.s_filterMatcherMap.TryGetValue(subscriptionFilter.Type, out str))
            tupleList.Add(Tuple.Create<string, string>(str, subscriptionFilter.EventType));
        }
        bool flag = limitToContributionNames != null && limitToContributionNames.Count > 0;
        foreach (Subscription contributedSubscription in contributedSubscriptions)
        {
          foreach (Tuple<string, string> tuple in tupleList)
          {
            string str1 = tuple.Item1;
            string str2 = tuple.Item2;
            if (str1.Equals(contributedSubscription.Matcher) && (string.IsNullOrEmpty(str2) || str2.Equals(contributedSubscription.SubscriptionFilter.EventType)) && (!flag || limitToContributionNames.Contains(contributedSubscription.SubscriptionId)))
            {
              source.Add(contributedSubscription);
              break;
            }
          }
        }
      }
      if (targetIdentity != null && targetIdentity.IsContainer)
      {
        List<Subscription> subscriptionList = new List<Subscription>();
        NotificationEventType eventType = (NotificationEventType) null;
        INotificationEventService service = requestContext.GetService<INotificationEventService>();
        foreach (Subscription subscription in source.OrderBy<Subscription, string>((Func<Subscription, string>) (s => s.SubscriptionFilter.EventType)).ToList<Subscription>())
        {
          if (eventType == null || eventType.Id != EventTypeMapper.ToContributed(requestContext, subscription.SubscriptionFilter.EventType))
            eventType = service.GetEventType(requestContext, subscription.SubscriptionFilter.EventType);
          if (!(targetIdentity.Id != subscription.SubscriberId) || NotificationSubscriptionService.IsGroupOptOutable(requestContext, subscription, subscription.SubscriberIdentity, eventType))
            subscriptionList.Add(subscription);
        }
        source = subscriptionList;
      }
      return (IEnumerable<Subscription>) source;
    }

    private SubscriptionLookup ParseQuery(
      IVssRequestContext requestContext,
      SubscriptionQueryCondition condition)
    {
      SubscriptionLookup query = SubscriptionLookup.CreateAnyFieldLookup();
      if (condition.Filter != null)
      {
        query = MatcherFilterFactory.GetMatcherFilter(requestContext, condition.Filter).ApplyToSubscriptionLookup(requestContext, condition);
        string str;
        if (NotificationSubscriptionService.s_filterMatcherMap.TryGetValue(condition.Filter.Type, out str))
          query.Matcher = str;
      }
      SubscriptionFlags? flags = condition.Flags;
      if (flags.HasValue)
        query.Flags = condition.Flags;
      Guid result1;
      if (condition.Scope != null && Guid.TryParse(condition.Scope, out result1))
        query.DataspaceId = new Guid?(result1);
      if (!string.IsNullOrEmpty(condition.SubscriptionId))
      {
        int result2;
        if (int.TryParse(condition.SubscriptionId, out result2))
        {
          query.SubscriptionId = new int?(result2);
        }
        else
        {
          Guid result3;
          if (Guid.TryParse(condition.SubscriptionId, out result3))
          {
            query.UniqueId = new Guid?(result3);
          }
          else
          {
            query.ContributedSubscriptionName = condition.SubscriptionId;
            SubscriptionLookup subscriptionLookup = query;
            flags = query.Flags;
            SubscriptionFlags? nullable;
            if (!flags.HasValue)
            {
              nullable = new SubscriptionFlags?(SubscriptionFlags.ContributedSubscription);
            }
            else
            {
              flags = query.Flags;
              nullable = flags.HasValue ? new SubscriptionFlags?(flags.GetValueOrDefault() | SubscriptionFlags.ContributedSubscription) : new SubscriptionFlags?();
            }
            subscriptionLookup.Flags = nullable;
          }
        }
      }
      Guid? subscriberId = condition.SubscriberId;
      if (subscriberId.HasValue)
      {
        subscriberId = condition.SubscriberId;
        if (subscriberId.Value != Guid.Empty)
          query.SubscriberId = condition.SubscriberId;
      }
      return query;
    }

    private Subscription GetContributedSubscription(
      IVssRequestContext requestContext,
      string subscriptionId)
    {
      List<string> contributionTargets = new List<string>()
      {
        NotificationClientConstants.DefaultSubscriptionContributionTarget,
        NotificationClientConstants.DefaultSystemSubscriptionContributionTarget
      };
      Subscription subscription = this.GetContributedSubscriptions(requestContext, (IEnumerable<string>) contributionTargets, (IEnumerable<string>) null, true).Where<Subscription>((Func<Subscription, bool>) (s => s.SubscriptionId == subscriptionId)).FirstOrDefault<Subscription>();
      if (subscription != null)
        this.PopulateUserSettings(requestContext, subscription);
      return subscription;
    }

    public IEnumerable<Subscription> GetContributedSubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionTargets,
      IEnumerable<string> allowedFilterTypes = null,
      bool forDisplay = true)
    {
      IContributionServiceWithData service1 = requestContext.GetService<IContributionServiceWithData>();
      INotificationEventService service2 = requestContext.GetService<INotificationEventService>();
      HashSet<string> contributionTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        NotificationClientConstants.SubscriptionContribution
      };
      ContributionQueryOptions queryOptions = ContributionQueryOptions.IncludeSubTree;
      string str = forDisplay ? "display" : "processing";
      string cacheKey = ContributionUtils.GetCacheKey(NotificationSubscriptionService.s_contributedSubscriptionsDataName + ":" + str, contributionTargets, contributionTypes, new ContributionQueryOptions?(queryOptions));
      IEnumerable<Contribution> contributions;
      Dictionary<string, List<Subscription>> associatedData;
      if (!service1.QueryContributions<Dictionary<string, List<Subscription>>>(requestContext, contributionTargets, contributionTypes, queryOptions, (ContributionQueryCallback) null, (ContributionDiagnostics) null, cacheKey, out contributions, out associatedData))
      {
        requestContext.WarnIfContributionsInFallbackMode(nameof (GetContributedSubscriptions));
        if (contributions != null && contributions.Any<Contribution>())
        {
          associatedData = new Dictionary<string, List<Subscription>>();
          foreach (Contribution contribution in contributions)
          {
            JObject properties = contribution.Properties;
            try
            {
              NotificationSubscriptionCreateParameters createParameters = properties.ToObject<NotificationSubscriptionCreateParameters>(this.JsonSerializer);
              if (createParameters.Filter == null)
                throw new InvalidContributedSubscriptionException("Missing filter Id:" + contribution.Id);
              if (string.IsNullOrEmpty(createParameters.Filter.EventType))
                throw new InvalidContributedSubscriptionException("Missing eventType Id:" + contribution.Id);
              NotificationEventType eventType = service2.GetEventType(requestContext, createParameters.Filter.EventType);
              if (eventType == null)
                requestContext.Trace(1002024, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), "Subscription with Id {0} has an eventtype {1} that is invalid or belongs to another service", (object) contribution.Id, (object) createParameters.Filter.EventType);
              else if (!this.IsMatchingServiceIntanceType(requestContext, eventType.EventPublisher))
              {
                requestContext.Trace(1002024, TraceLevel.Verbose, "Notifications", nameof (NotificationSubscriptionService), "Subscription with Id {0} belongs to another service", (object) contribution.Id);
              }
              else
              {
                if (string.IsNullOrWhiteSpace(createParameters.Description))
                  createParameters.Description = contribution.Description;
                if (!forDisplay)
                  MatcherFilterFactory.GetMatcherFilter(requestContext, createParameters.Filter).PrepareForDisplay(requestContext, createParameters.Filter);
                Subscription subscription1 = this.PrepareForCreate(requestContext, createParameters, false);
                if (forDisplay)
                  MatcherFilterFactory.GetMatcherFilter(requestContext, subscription1.SubscriptionFilter).PrepareForDisplay(requestContext, subscription1.SubscriptionFilter);
                subscription1.ExtendedProperties = contribution.GetProperty<Dictionary<string, string>>(NotificationClientConstants.DetailedDescriptionProperty);
                subscription1.ContributionId = contribution.Id;
                subscription1.ToLegacyEventType(requestContext);
                subscription1.IsSystem = contribution.Targets.Contains(NotificationClientConstants.DefaultSystemSubscriptionContributionTarget);
                subscription1.SubscriberIdentity = this.GetValidUsersGroup(requestContext);
                Subscription subscription2 = subscription1;
                Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity = subscription1.SubscriberIdentity;
                Guid guid = subscriberIdentity != null ? subscriberIdentity.Id : Guid.Empty;
                subscription2.SubscriberId = guid;
                subscription1.Flags |= SubscriptionFlags.GroupSubscription | SubscriptionFlags.ContributedSubscription;
                if (!subscription1.IsSystem)
                  subscription1.Flags |= SubscriptionFlags.CanOptOut;
                subscription1.Permissions = SubscriptionPermissions.View;
                if (string.IsNullOrEmpty(subscription1.Channel))
                {
                  subscription1.Channel = "User";
                }
                else
                {
                  string channel = subscription1.Channel;
                  if (!(channel == "User") && !(channel == "UserSystem"))
                    throw new InvalidContributedSubscriptionException("Unsupported Channel:" + subscription1.Channel + " Id:" + contribution.Id);
                }
                string type = subscription1.SubscriptionFilter.Type;
                if (associatedData.ContainsKey(type))
                  associatedData[type].Add(subscription1);
                else
                  associatedData.Add(type, new List<Subscription>()
                  {
                    subscription1
                  });
              }
            }
            catch (InvalidContributedSubscriptionException ex)
            {
              requestContext.TraceException(1002022, "Notifications", nameof (NotificationSubscriptionService), (Exception) ex);
            }
            catch (Exception ex)
            {
              Exception exception = (Exception) new InvalidContributedSubscriptionException("Id: " + contribution.Id + " Inner:" + ex.GetType().Name, ex);
              requestContext.TraceException(1002022, "Notifications", nameof (NotificationSubscriptionService), exception);
            }
          }
          service1.Set(requestContext, cacheKey, contributions, (object) associatedData);
        }
      }
      List<Subscription> contributedSubscriptions = new List<Subscription>();
      if (associatedData != null)
      {
        foreach (string key in associatedData.Keys)
        {
          if (allowedFilterTypes == null || allowedFilterTypes.Contains<string>(key))
            contributedSubscriptions.AddRange((IEnumerable<Subscription>) associatedData[key].Select<Subscription, Subscription>((Func<Subscription, Subscription>) (x => (Subscription) x.Clone())).ToList<Subscription>());
        }
      }
      this.PopulateAdminSettings(requestContext, (IEnumerable<Subscription>) contributedSubscriptions);
      this.PopulateDiagnosticsForContributedSubscriptions(requestContext, (IEnumerable<Subscription>) contributedSubscriptions);
      return (IEnumerable<Subscription>) contributedSubscriptions;
    }

    public IEnumerable<Subscription> GetSubscriptionsForTarget(
      IVssRequestContext requestContext,
      Guid targetId,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None)
    {
      List<Subscription> subscriptionsForTarget = new List<Subscription>();
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity1 = this.ValidateTargetIdentity(requestContext, targetId);
      if (queryFlags.HasFlag((Enum) SubscriptionQueryFlags.AlwaysReturnBasicInformation))
        throw new InvalidQueryFlagException(CoreRes.InvalidQueryFlag());
      subscriptionsForTarget.AddRange((IEnumerable<Subscription>) this.QueryCustomSubscriptions(requestContext, (IEnumerable<SubscriptionLookup>) new List<SubscriptionLookup>()
      {
        SubscriptionLookup.CreateForTargetLookup(targetIdentity1.Id)
      }, queryFlags, targetIdentity: targetIdentity1));
      IVssRequestContext requestContext1 = requestContext;
      List<string> contributionTargets = new List<string>();
      contributionTargets.Add(NotificationClientConstants.DefaultSubscriptionContributionTarget);
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity2 = targetIdentity1;
      subscriptionsForTarget.AddRange(this.QueryContributedSubscriptions(requestContext1, (IEnumerable<string>) contributionTargets, targetIdentity: targetIdentity2));
      return (IEnumerable<Subscription>) subscriptionsForTarget;
    }

    public IEnumerable<NotificationSubscription> GetNotificationSubscriptionsForTarget(
      IVssRequestContext requestContext,
      Guid targetId,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None)
    {
      IEnumerable<Subscription> subscriptionsForTarget = this.GetSubscriptionsForTarget(requestContext, targetId, queryFlags);
      return this.ToNotificationSubscriptions(requestContext, subscriptionsForTarget, queryFlags).Where<NotificationSubscription>((Func<NotificationSubscription, bool>) (subscription => BaseMatcherFilter.AllowedChannelsForCustomSubscriptions.Contains(subscription.Channel.Type)));
    }

    private IEnumerable<Subscription> QueryContributedSubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionTargets,
      List<ISubscriptionFilter> filtersForContributed = null,
      HashSet<string> limitToContributionNames = null,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = null)
    {
      IEnumerable<Subscription> subscriptions;
      using (PerformanceTimer.StartMeasure(requestContext, "QueryContributedSubscritptions_GetContributedSubscriptions"))
        subscriptions = this.GetContributedSubscriptions(requestContext, contributionTargets, (IEnumerable<string>) null, true);
      using (PerformanceTimer.StartMeasure(requestContext, "QueryContributedSubscritptions_FilterContributedSubscriptionsForQuery"))
        subscriptions = this.FilterContributedSubscriptionsForQuery(requestContext, subscriptions, filtersForContributed, limitToContributionNames, targetIdentity);
      using (PerformanceTimer.StartMeasure(requestContext, "QueryContributedSubscritptions_PopulateUserSettings"))
        this.PopulateUserSettings(requestContext, subscriptions, targetIdentity);
      return subscriptions;
    }

    private Microsoft.VisualStudio.Services.Identity.Identity ValidateTargetIdentity(
      IVssRequestContext requestContext,
      Guid targetId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      if (targetId.Equals(Guid.Empty))
        identity = requestContext.GetUserIdentity();
      else
        identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
        {
          targetId
        }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>();
      return identity != null ? identity : throw new IdentityNotFoundException(targetId);
    }

    private bool IsMatchingServiceIntanceType(
      IVssRequestContext requestContext,
      NotificationEventPublisher eventsPublisher)
    {
      return !this.IsHosted(requestContext) || eventsPublisher.SubscriptionManagementInfo.ServiceInstanceType.Equals(Guid.Empty) || this.GetCurrentServiceInstanceType(requestContext).Equals(eventsPublisher.SubscriptionManagementInfo.ServiceInstanceType);
    }

    private Guid GetCurrentServiceInstanceType(IVssRequestContext requestContext) => requestContext.ServiceInstanceType();

    private bool IsHosted(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment;

    internal static string GetPathMatcherForEvent(
      IVssRequestContext requestContext,
      string eventType)
    {
      return requestContext.GetService<NotificationEventService>().GetSerializationFormatForEvent(requestContext, eventType) != EventSerializerType.Json ? "XPathMatcher" : "JsonPathMatcher";
    }

    public JsonSerializer JsonSerializer
    {
      get
      {
        if (this.s_jsonSerializer == null)
        {
          JsonSerializer jsonSerializer = new JsonSerializer();
          jsonSerializer.ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver();
          jsonSerializer.Converters.Add((JsonConverter) new StringEnumConverter()
          {
            NamingStrategy = (NamingStrategy) new CamelCaseNamingStrategy()
          });
          this.s_jsonSerializer = jsonSerializer;
        }
        return this.s_jsonSerializer;
      }
    }

    public IEnumerable<NotificationSubscriptionTemplate> GetSubscriptionTemplates(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      SubscriptionTemplateQueryFlags queryFlags = SubscriptionTemplateQueryFlags.IncludeUserAndGroup)
    {
      IContributionServiceWithData service1 = requestContext.GetService<IContributionServiceWithData>();
      INotificationEventService service2 = requestContext.GetService<INotificationEventService>();
      List<string> subscriptionTemplateTargets = this.getSubscriptionTemplateTargets(queryFlags);
      ContributionQueryOptions queryOptions = ContributionQueryOptions.IncludeChildren;
      HashSet<string> contributionTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        "ms.vss-notifications.subscription-template"
      };
      string cacheKey = ContributionUtils.GetCacheKey(NotificationSubscriptionService.s_subscriptionTemplatesDataName, (IEnumerable<string>) subscriptionTemplateTargets, contributionTypes, new ContributionQueryOptions?(queryOptions));
      IEnumerable<Contribution> contributions;
      List<NotificationSubscriptionTemplate> associatedData;
      if (!service1.QueryContributions<List<NotificationSubscriptionTemplate>>(requestContext, (IEnumerable<string>) subscriptionTemplateTargets, contributionTypes, queryOptions, (ContributionQueryCallback) null, (ContributionDiagnostics) null, cacheKey, out contributions, out associatedData))
      {
        Dictionary<string, List<NotificationSubscriptionTemplate>> dictionary = new Dictionary<string, List<NotificationSubscriptionTemplate>>();
        associatedData = new List<NotificationSubscriptionTemplate>();
        requestContext.WarnIfContributionsInFallbackMode(nameof (GetSubscriptionTemplates));
        if (contributions.Any<Contribution>())
        {
          foreach (Contribution contribution in contributions)
          {
            JObject properties = contribution.Properties;
            try
            {
              NotificationSubscriptionTemplate subscriptionTemplate = properties.ToObject<NotificationSubscriptionTemplate>(this.JsonSerializer);
              subscriptionTemplate.Type = this.getTemplateType(contribution);
              subscriptionTemplate.Id = contribution.Id;
              if (subscriptionTemplate.Filter == null)
                requestContext.Trace(1002022, TraceLevel.Error, "Notifications", nameof (NotificationSubscriptionService), "Missing filter for subscription template {0}", (object) contribution.Id);
              else if (string.IsNullOrEmpty(subscriptionTemplate.Filter.EventType))
              {
                requestContext.Trace(1002022, TraceLevel.Error, "Notifications", nameof (NotificationSubscriptionService), "Missing eventtype on filter for subscription template {0}", (object) contribution.Id);
              }
              else
              {
                SubscriptionAdapterFactory.CreateAdapter(requestContext, subscriptionTemplate.Filter, new SubscriptionScope()).PrepareForDisplay(requestContext, subscriptionTemplate.Filter);
                string eventType = subscriptionTemplate.Filter.EventType;
                if (!dictionary.ContainsKey(eventType))
                  dictionary.Add(eventType, new List<NotificationSubscriptionTemplate>());
                dictionary[eventType].Add(subscriptionTemplate);
              }
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1002022, "Notifications", nameof (NotificationSubscriptionService), ex);
            }
          }
          bool flag = queryFlags.HasFlag((Enum) SubscriptionTemplateQueryFlags.IncludeEventTypeInformation);
          Dictionary<string, NotificationEventType> mapEventTypes = service2.GetEventTypes(requestContext).ToDictionary<NotificationEventType, string, NotificationEventType>((Func<NotificationEventType, string>) (i => i.Id), (Func<NotificationEventType, NotificationEventType>) (i => i));
          foreach (string key in dictionary.Keys)
          {
            string eventTypeId = key;
            if (mapEventTypes.ContainsKey(eventTypeId))
            {
              if (flag)
                dictionary[eventTypeId].ForEach((Action<NotificationSubscriptionTemplate>) (t => t.NotificationEventInformation = mapEventTypes[eventTypeId]));
              associatedData.AddRange((IEnumerable<NotificationSubscriptionTemplate>) dictionary[eventTypeId]);
            }
          }
        }
        service1.Set(requestContext, cacheKey, contributions, (object) associatedData);
      }
      return (IEnumerable<NotificationSubscriptionTemplate>) associatedData;
    }

    private bool CreateOrUpdateSubscriptionAllowed(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity,
      string matcher)
    {
      if ("BlockMatcher".Equals(matcher, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(CoreRes.BlockFilterNotSupportException());
      return NotificationSubscriptionSecurityUtils.HasPermissions(requestContext, subscriberIdentity, 2);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity GetValidUsersGroup(
      IVssRequestContext requestContext)
    {
      if (this.m_projectCollectionValidUsers == null)
        this.m_projectCollectionValidUsers = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
        {
          GroupWellKnownIdentityDescriptors.EveryoneGroup
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
      return this.m_projectCollectionValidUsers;
    }

    private void SetSubscriptionTemplateEventType(
      List<NotificationSubscriptionTemplate> subscriptionTemplates,
      NotificationEventType eventType)
    {
      foreach (NotificationSubscriptionTemplate subscriptionTemplate in subscriptionTemplates)
        subscriptionTemplate.NotificationEventInformation = eventType;
    }

    private List<string> getSubscriptionTemplateTargets(SubscriptionTemplateQueryFlags queryFlags)
    {
      List<string> subscriptionTemplateTargets = new List<string>();
      if (queryFlags.HasFlag((Enum) SubscriptionTemplateQueryFlags.IncludeUserAndGroup))
      {
        subscriptionTemplateTargets.Add("ms.vss-notifications.user-subscription-templates");
        subscriptionTemplateTargets.Add("ms.vss-notifications.team-subscription-templates");
      }
      else if (queryFlags.HasFlag((Enum) SubscriptionTemplateQueryFlags.IncludeUser))
        subscriptionTemplateTargets.Add("ms.vss-notifications.user-subscription-templates");
      else if (queryFlags.HasFlag((Enum) SubscriptionTemplateQueryFlags.IncludeGroup))
        subscriptionTemplateTargets.Add("ms.vss-notifications.team-subscription-templates");
      return subscriptionTemplateTargets;
    }

    private SubscriptionTemplateType getTemplateType(Contribution contribution)
    {
      SubscriptionTemplateType templateType = SubscriptionTemplateType.None;
      if (contribution.Targets != null && contribution.Targets.Count > 0)
      {
        if (contribution.Targets.Contains("ms.vss-notifications.user-subscription-templates") && contribution.Targets.Contains("ms.vss-notifications.team-subscription-templates"))
          templateType = SubscriptionTemplateType.Both;
        else if (contribution.Targets.Contains("ms.vss-notifications.user-subscription-templates"))
          templateType = SubscriptionTemplateType.User;
        else if (contribution.Targets.Contains("ms.vss-notifications.team-subscription-templates"))
          templateType = SubscriptionTemplateType.Team;
      }
      return templateType;
    }

    private bool IsContributedEventType(IVssRequestContext requestContext, string eventType)
    {
      Contribution contribution = requestContext.GetService<IContributionService>().QueryContribution(requestContext, eventType);
      if (contribution == null)
        return false;
      this.m_supportedEventTypes.Add(this.GetContributionId(contribution.Id));
      return true;
    }

    private string GetContributionId(string contributionId) => string.IsNullOrEmpty(contributionId) ? string.Empty : new ContributionIdentifier(contributionId).RelativeId;

    private bool VerifyEventType(IVssRequestContext requestContext, string eventType) => EventTypeMapper.IsLegacy(requestContext, eventType) || this.m_supportedEventTypes.Contains(eventType) || this.IsContributedEventType(requestContext, eventType);

    public void UpdateSubscriptionProject(
      IVssRequestContext requestContext,
      Subscription subscription,
      Guid projectId)
    {
      MatcherFilterFactory.GetMatcherFilter(requestContext, subscription.Matcher).PreBindSubscription(requestContext, subscription);
      if (!requestContext.GetService<INotificationEventService>().IsValidEventType(requestContext, subscription.EventTypeName))
        throw new EventTypeDoesNotExistException(subscription.EventTypeName);
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
        component.UpdateSubscriptionProject(subscription, projectId);
    }

    public static string GetSubscriptionRestURL(
      IVssRequestContext requestContext,
      string subscriptionId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "notification", NotificationApiConstants.SubscriptionsResource.LocationId, (object) new
        {
          subscriptionId = subscriptionId,
          resource = "Subscriptions"
        }).AbsoluteUri.ToString();
      }
      catch (Exception ex)
      {
      }
      return string.Empty;
    }

    public static string GetDiagnosticLogRestURL(
      IVssRequestContext requestContext,
      Guid source,
      Guid instanceId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "notification", NotificationApiConstants.NotificationDiagnosticLogsResource.LocationId, (object) new
        {
          source = source,
          entryId = instanceId,
          resource = "DiagnosticLogs"
        }).AbsoluteUri.ToString();
      }
      catch (Exception ex)
      {
      }
      return string.Empty;
    }

    public static string GetMyNotificationsUrl(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      string notificationsUrl = (string) null;
      if (requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
      {
        string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, requestContext.ExecutionEnvironment.IsHostedDeployment ? ServiceInstanceTypes.TFS : LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
        if (!string.IsNullOrEmpty(locationServiceUrl))
        {
          notificationsUrl = UriUtility.CombinePath(locationServiceUrl, "_notifications") + "?" + "subscriptionId" + "=" + Uri.EscapeDataString(subscription.SubscriptionId);
          string eventType1 = subscription.SubscriptionFilter?.EventType;
          if (!string.IsNullOrEmpty(eventType1))
          {
            NotificationEventType eventType2 = requestContext.GetService<INotificationEventService>().GetEventType(requestContext, eventType1);
            if (eventType2 != null && eventType2.EventPublisher != null)
              notificationsUrl = notificationsUrl + "&" + "publisherId" + "=" + Uri.EscapeDataString(eventType2.EventPublisher.Id);
          }
        }
      }
      return notificationsUrl;
    }

    private static bool IsGroupOptOutable(
      IVssRequestContext requestContext,
      Subscription subscription,
      Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity,
      NotificationEventType eventType)
    {
      if (eventType == null || (subscriberIdentity == null ? 0 : (subscriberIdentity.IsContainer ? 1 : 0)) == 0)
        return false;
      if (NotificationSubscriptionService.IsPathMatcher(subscription.Matcher) && subscription.IsOptOutable())
        return true;
      if (NotificationSubscriptionService.IsActorMatcher(subscription.Matcher) && subscription.SubscriptionFilter is RoleBasedFilter subscriptionFilter)
      {
        foreach (string inclusion1 in (IEnumerable<string>) subscriptionFilter.Inclusions)
        {
          string inclusion = inclusion1;
          NotificationEventRole notificationEventRole = eventType.Roles.Where<NotificationEventRole>((Func<NotificationEventRole, bool>) (s => s.Id.Equals(inclusion))).FirstOrDefault<NotificationEventRole>();
          if (notificationEventRole != null && notificationEventRole.SupportsGroups)
            return true;
        }
      }
      return false;
    }

    public static bool IsPathMatcher(string matcher)
    {
      if (string.IsNullOrEmpty(matcher))
        return false;
      return matcher.Equals("XPathMatcher") || matcher.Equals("JsonPathMatcher") || matcher.Equals("PathMatcher");
    }

    public static bool IsActorMatcher(string matcher) => !string.IsNullOrEmpty(matcher) && matcher.Equals("ActorMatcher");

    public static bool IsJsonPathCompatibleMatcher(string matcher)
    {
      if (string.IsNullOrEmpty(matcher))
        return false;
      return matcher.Equals("JsonPathMatcher") || matcher.Equals("PathMatcher") || NotificationSubscriptionService.IsActorMatcher(matcher);
    }

    public static bool IsXPathCompatibleMatcher(string matcher)
    {
      if (string.IsNullOrEmpty(matcher))
        return false;
      return matcher.Equals("XPathMatcher") || matcher.Equals("PathMatcher") || NotificationSubscriptionService.IsActorMatcher(matcher);
    }

    public static bool IsEventTypeRequiredMatcher(string matcher) => !string.IsNullOrEmpty(matcher) && !matcher.Equals("FollowsMatcher");

    public bool IsJobRegistered(IVssRequestContext requestContext, Guid jobId)
    {
      JobRegisteredStatus registeredStatus;
      if (!this.m_jobRegistrations.TryGetValue(jobId, out registeredStatus))
      {
        registeredStatus = new JobRegisteredStatus(jobId);
        this.m_jobRegistrations[jobId] = registeredStatus;
      }
      return registeredStatus.IsJobRegistered(requestContext, this.m_jobRegisteredCheckFrequency);
    }

    public HashSet<string> GetAllowedChannelsForCustomSubscriptions(
      IVssRequestContext requestContext)
    {
      return new HashSet<string>((IEnumerable<string>) BaseMatcherFilter.AllowedChannelsForCustomSubscriptions);
    }

    public static StringFieldMode GetStringFieldMode(IVssRequestContext requestContext) => StringFieldMode.Optimized;

    internal class IdentityPermissions
    {
      public Microsoft.VisualStudio.Services.Identity.Identity Identity;
      public SubscriptionPermissions Permissions;
      public bool IsMember;
    }
  }
}
