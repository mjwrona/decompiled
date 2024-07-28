// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationAuditing
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class NotificationAuditing
  {
    internal static readonly string SubscriptionIdProperty = "subscriptionId";
    internal static readonly string IsContributedProperty = "subscriptionIsContributed";
    internal static readonly string LastModifiedByProperty = "lastModifiedBy";
    internal static readonly string OldLastModifiedByProperty = "oldLastModifiedBy";
    internal static readonly string CustomDeliveryAddressProperty = "customDeliveryAddress";
    internal static readonly string SubscriberIdProperty = "subscriberId";
    internal static readonly string SubscriberEmailProperty = "subscriberEmail";
    internal static readonly string SubscriberDeliveryPreferenceProperty = "subscriberDeliveryPreference";
    internal static readonly string UserIdProperty = "userId";
    internal static readonly string SubscriberIsGroupProperty = "subscriberIsGroup";
    internal static readonly string SubscriberIsDefaultUserGroupProperty = "subscriberIsDefaultUserGroup";
    internal static readonly string StatusProperty = "status";
    internal static readonly string OldStatusProperty = "oldStatus";
    internal static readonly string NewStatusProperty = "newStatus";
    internal static readonly string OldBlockUserOptOutProperty = "oldBlockUserOptOut";
    internal static readonly string NewBlockUserOptOutProperty = "newBlockUserOptOut";
    internal static readonly string ScopeIdProperty = "scopeId";
    internal static readonly string OldScopeIdProperty = "oldScopeId";
    internal static readonly string MatcherTypeProperty = "matcherType";
    internal static readonly string OldMatcherTypeProperty = "oldMatcherType";
    internal static readonly string EventTypeProperty = "eventType";
    internal static readonly string OldEventTypeProperty = "oldEventType";
    internal static readonly string ChannelTypeProperty = "channelType";
    internal static readonly string OldChannelTypeProperty = "oldChannelType";
    internal static readonly string ChangesProperty = "changes";
    internal static readonly string SubscriptionExpressionProperty = "expression";
    internal static readonly string ArtifactTypeProperty = "artifactType";
    internal static readonly string DefaultGroupDeliveryPreferenceProperty = "defaultGroupDeliveryPreference";
    internal static readonly string OldDefaultGroupDeliveryPreferenceProperty = "oldDefaultGroupDeliveryPreference";
    internal static readonly string SubscriptionDescriptionProperty = "description";
    internal static readonly string ProjectNameProperty = "projectName";
    internal static readonly string AdminSettingChangeMessageProperty = "settingChange";
    private const string s_area = "Notifications";
    private const string s_layer = "NotificationClientTrace";

    private static void PublishEvent(
      IVssRequestContext requestContext,
      string feature,
      string action,
      ClientTraceData ctData)
    {
      try
      {
        ClientTraceService service = requestContext.GetService<ClientTraceService>();
        ctData.Add(CustomerIntelligenceProperty.Action, (object) action);
        IVssRequestContext requestContext1 = requestContext;
        string notifications = CustomerIntelligenceArea.Notifications;
        string feature1 = feature;
        ClientTraceData properties = ctData;
        service.Publish(requestContext1, notifications, feature1, properties);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1002031, TraceLevel.Error, "Notifications", "NotificationClientTrace", "Exception: " + ex.GetType().Name + " publishing for " + feature + ":" + action);
      }
    }

    internal static void PublishCreateSubscriptionEvent(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      ClientTraceData ctData = (ClientTraceData) null;
      if (!NotificationAuditing.TryGetSubcriptionEventData(requestContext, subscription, out ctData))
        return;
      NotificationAuditing.AddWorkItemSubscriptionData(subscription, ctData);
      NotificationAuditing.PublishEvent(requestContext, NotificationAuditingConstants.SubscriptionManagement, NotificationAuditingConstants.CreateSubscriptionAction, ctData);
    }

    internal static void PublishDeleteSubscriptionEvent(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      ClientTraceData ctData = (ClientTraceData) null;
      if (!NotificationAuditing.TryGetSubcriptionEventData(requestContext, subscription, out ctData))
        return;
      NotificationAuditing.PublishEvent(requestContext, NotificationAuditingConstants.SubscriptionManagement, NotificationAuditingConstants.DeleteSubscriptionAction, ctData);
    }

    internal static void PublishUpdateSubscriptionEvent(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      SubscriptionUpdate subscriptionUpdate)
    {
      ClientTraceData ctData = (ClientTraceData) null;
      if (!NotificationAuditing.TryGetSubcriptionEventData(requestContext, originalSubscription, out ctData))
        return;
      List<string> source = new List<string>();
      if (subscriptionUpdate.ScopeId.HasValue && !subscriptionUpdate.ScopeId.Equals((object) originalSubscription.ScopeId))
      {
        ctData.GetData()[NotificationAuditing.ScopeIdProperty] = (object) subscriptionUpdate.ScopeId;
        ctData.Add(NotificationAuditing.OldScopeIdProperty, (object) originalSubscription.ScopeId);
        source.Add(NotificationAuditing.ScopeIdProperty);
      }
      if (subscriptionUpdate.Status.HasValue)
      {
        int status1 = (int) originalSubscription.Status;
        SubscriptionStatus? status2 = subscriptionUpdate.Status;
        int valueOrDefault = (int) status2.GetValueOrDefault();
        if (!(status1 == valueOrDefault & status2.HasValue))
        {
          ctData.GetData()[NotificationAuditing.StatusProperty] = (object) subscriptionUpdate.Status;
          ctData.Add(NotificationAuditing.OldStatusProperty, (object) originalSubscription.Status);
          source.Add(NotificationAuditing.StatusProperty);
        }
      }
      if (subscriptionUpdate.EventTypeName != null && originalSubscription.SubscriptionFilter != null && !subscriptionUpdate.EventTypeName.Equals(originalSubscription.SubscriptionFilter.EventType))
      {
        ctData.GetData()[NotificationAuditing.EventTypeProperty] = (object) EventTypeMapper.ToContributed(requestContext, subscriptionUpdate.EventTypeName);
        ctData.Add(NotificationAuditing.OldEventTypeProperty, (object) EventTypeMapper.ToContributed(requestContext, originalSubscription.SubscriptionFilter.EventType));
        source.Add(NotificationAuditing.EventTypeProperty);
      }
      if (subscriptionUpdate.Matcher != null && !subscriptionUpdate.Matcher.Equals(originalSubscription.Matcher))
      {
        ctData.GetData()[NotificationAuditing.MatcherTypeProperty] = (object) subscriptionUpdate.Matcher;
        ctData.Add(NotificationAuditing.OldMatcherTypeProperty, (object) originalSubscription.Matcher);
        source.Add(NotificationAuditing.MatcherTypeProperty);
      }
      if (subscriptionUpdate.Channel != null && !subscriptionUpdate.Channel.Equals(originalSubscription.Channel))
      {
        ctData.GetData()[NotificationAuditing.ChannelTypeProperty] = (object) subscriptionUpdate.Channel;
        ctData.Add(NotificationAuditing.OldChannelTypeProperty, (object) originalSubscription.Channel);
        source.Add(NotificationAuditing.ChannelTypeProperty);
      }
      if (subscriptionUpdate.LastModifiedBy.HasValue && !subscriptionUpdate.LastModifiedBy.Equals((object) originalSubscription.LastModifiedBy))
      {
        ctData.GetData()[NotificationAuditing.LastModifiedByProperty] = (object) subscriptionUpdate.LastModifiedBy;
        ctData.Add(NotificationAuditing.OldLastModifiedByProperty, (object) originalSubscription.LastModifiedBy);
        source.Add(NotificationAuditing.LastModifiedByProperty);
      }
      if (source.Any<string>())
        ctData.Add(NotificationAuditing.ChangesProperty, (object) source);
      NotificationAuditing.AddWorkItemSubscriptionData(originalSubscription, ctData);
      NotificationAuditing.PublishEvent(requestContext, NotificationAuditingConstants.SubscriptionManagement, NotificationAuditingConstants.UpdateSubscriptionAction, ctData);
    }

    internal static void PublishUpdateSubscriptionUserSettingsEvent(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionUserSettings userSettings)
    {
      ClientTraceData ctData = (ClientTraceData) null;
      if (!NotificationAuditing.TryGetSubcriptionEventData(requestContext, subscription, out ctData))
        return;
      string action = userSettings.OptedOut ? NotificationAuditingConstants.UserOptOutAction : NotificationAuditingConstants.UserOptInAction;
      NotificationAuditing.PublishEvent(requestContext, NotificationAuditingConstants.SubscriptionManagement, action, ctData);
    }

    internal static void PublishUpdateSubscriptionAdminSettingsEvent(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      SubscriptionStatus? newStatus,
      bool? newBlockUserOptOut)
    {
      ClientTraceData ctData = new ClientTraceData();
      StringBuilder stringBuilder = new StringBuilder();
      ctData.Add(NotificationAuditing.SubscriptionDescriptionProperty, (object) originalSubscription.Description);
      ctData.Add(NotificationAuditing.SubscriptionIdProperty, (object) originalSubscription.SubscriptionId);
      SubscriptionStatus status = originalSubscription.Status;
      ctData.Add(NotificationAuditing.OldStatusProperty, (object) status);
      ctData.Add(NotificationAuditing.NewStatusProperty, (object) newStatus);
      if (newStatus.HasValue)
      {
        int num = (int) status;
        SubscriptionStatus? nullable = newStatus;
        int valueOrDefault = (int) nullable.GetValueOrDefault();
        if (!(num == valueOrDefault & nullable.HasValue))
          stringBuilder.Append(CoreRes.StatusChangeAuditMessage((object) status, (object) newStatus));
      }
      bool settingsBlockUserOptOut = originalSubscription.AdminSettingsBlockUserOptOut;
      ctData.Add(NotificationAuditing.OldBlockUserOptOutProperty, (object) settingsBlockUserOptOut);
      ctData.Add(NotificationAuditing.NewBlockUserOptOutProperty, (object) newBlockUserOptOut);
      if (newBlockUserOptOut.HasValue)
      {
        int num1 = settingsBlockUserOptOut ? 1 : 0;
        bool? nullable = newBlockUserOptOut;
        int num2 = nullable.GetValueOrDefault() ? 1 : 0;
        if (!(num1 == num2 & nullable.HasValue))
        {
          if (stringBuilder.Length != 0)
            stringBuilder.Append(" ");
          stringBuilder.Append(CoreRes.BlockUserOptOutAuditMessage((object) settingsBlockUserOptOut, (object) newBlockUserOptOut));
        }
      }
      ctData.Add(NotificationAuditing.AdminSettingChangeMessageProperty, (object) stringBuilder.ToString());
      NotificationAuditing.PublishEvent(requestContext, NotificationAuditingConstants.SubscriptionManagement, NotificationAuditingConstants.AdminSettingsAction, ctData);
    }

    private static bool TryGetSubcriptionEventData(
      IVssRequestContext requestContext,
      Subscription subscription,
      out ClientTraceData ctData)
    {
      try
      {
        ctData = new ClientTraceData();
        ctData.Add(NotificationAuditing.ProjectNameProperty, (object) subscription.ProjectName);
        ctData.Add(NotificationAuditing.SubscriptionDescriptionProperty, (object) subscription.Description);
        ctData.Add(NotificationAuditing.SubscriptionIdProperty, (object) subscription.SubscriptionId);
        ctData.Add(NotificationAuditing.IsContributedProperty, (object) subscription.IsContributed);
        ctData.Add(NotificationAuditing.StatusProperty, (object) subscription.Status);
        ctData.Add(NotificationAuditing.LastModifiedByProperty, (object) subscription.LastModifiedBy);
        ctData.Add(NotificationAuditing.CustomDeliveryAddressProperty, (object) !string.IsNullOrEmpty(subscription.DeliveryAddress));
        Microsoft.VisualStudio.Services.Identity.Identity identity = subscription.SubscriberIdentity;
        if (identity == null)
        {
          Guid subscriberId = subscription.SubscriberId;
          identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
          {
            subscription.SubscriberId
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
        }
        if (identity != null)
        {
          ctData.Add(NotificationAuditing.SubscriberIdProperty, (object) identity.Id);
          ctData.Add(NotificationAuditing.SubscriberIsGroupProperty, (object) identity.IsContainer);
          if (identity.IsContainer)
          {
            INotificationSubscriptionServiceInternal service = requestContext.GetService<INotificationSubscriptionServiceInternal>();
            bool flag = IdentityDescriptorComparer.Instance.Compare(identity.Descriptor, service.GetValidUsersGroup(requestContext).Descriptor) == 0;
            ctData.Add(NotificationAuditing.SubscriberIsDefaultUserGroupProperty, (object) flag);
          }
        }
        ctData.Add(NotificationAuditing.ScopeIdProperty, (object) subscription.ScopeId);
        ctData.Add(NotificationAuditing.MatcherTypeProperty, (object) subscription.Matcher);
        if (subscription.SubscriptionFilter is ArtifactFilter)
          ctData.Add(NotificationAuditing.ArtifactTypeProperty, (object) ((ArtifactFilter) subscription.SubscriptionFilter).ArtifactType);
        if (subscription.SubscriptionFilter != null)
          ctData.Add(NotificationAuditing.EventTypeProperty, (object) EventTypeMapper.ToContributed(requestContext, subscription.SubscriptionFilter.EventType));
        ctData.Add(NotificationAuditing.ChannelTypeProperty, (object) subscription.Channel);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002030, "Notifications", "NotificationClientTrace", ex);
        ctData = (ClientTraceData) null;
        return false;
      }
    }

    internal static void PublishUpdateSubscriberEvent(
      IVssRequestContext requestContext,
      Guid subscriberId,
      NotificationSubscriberUpdateParameters updateParameters)
    {
      ClientTraceData ctData = (ClientTraceData) null;
      if (!NotificationAuditing.TryGetUpdateSubscriberEventData(requestContext, subscriberId, updateParameters, out ctData))
        return;
      NotificationAuditing.PublishEvent(requestContext, NotificationAuditingConstants.SubscriberManagement, NotificationAuditingConstants.UpdateSubscriberAction, ctData);
    }

    internal static void PublishUpdateAdminSettingsEvent(
      IVssRequestContext requestContext,
      NotificationAdminSettings originalSettings,
      NotificationAdminSettings newSettings)
    {
      ClientTraceData ctData = (ClientTraceData) null;
      if (!NotificationAuditing.TryGetUpdateAdminSettingsEventData(requestContext, originalSettings, newSettings, out ctData))
        return;
      NotificationAuditing.PublishEvent(requestContext, NotificationAuditingConstants.AdminSettingsManagement, NotificationAuditingConstants.UpdateAdminSettingsAction, ctData);
    }

    private static bool TryGetUpdateSubscriberEventData(
      IVssRequestContext requestContext,
      Guid subscriberId,
      NotificationSubscriberUpdateParameters updateParameters,
      out ClientTraceData ctData)
    {
      try
      {
        ctData = new ClientTraceData();
        ctData.Add(NotificationAuditing.SubscriberIdProperty, (object) subscriberId);
        ctData.Add(NotificationAuditing.SubscriberEmailProperty, (object) (updateParameters.PreferredEmailAddress ?? "(empty)"));
        ClientTraceData clientTraceData = ctData;
        string preferenceProperty = NotificationAuditing.SubscriberDeliveryPreferenceProperty;
        NotificationSubscriberDeliveryPreference? deliveryPreference = updateParameters.DeliveryPreference;
        ref NotificationSubscriberDeliveryPreference? local = ref deliveryPreference;
        string str = (local.HasValue ? local.GetValueOrDefault().ToString() : (string) null) ?? "(empty)";
        clientTraceData.Add(preferenceProperty, (object) str);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002030, "Notifications", "NotificationClientTrace", ex);
        ctData = (ClientTraceData) null;
        return false;
      }
    }

    private static bool TryGetUpdateAdminSettingsEventData(
      IVssRequestContext requestContext,
      NotificationAdminSettings originalSettings,
      NotificationAdminSettings newSettings,
      out ClientTraceData ctData)
    {
      try
      {
        ctData = new ClientTraceData();
        ctData.Add(NotificationAuditing.OldDefaultGroupDeliveryPreferenceProperty, (object) originalSettings.DefaultGroupDeliveryPreference);
        ctData.Add(NotificationAuditing.DefaultGroupDeliveryPreferenceProperty, (object) newSettings.DefaultGroupDeliveryPreference);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002030, "Notifications", "NotificationClientTrace", ex);
        ctData = (ClientTraceData) null;
        return false;
      }
    }

    private static void AddWorkItemSubscriptionData(
      Subscription subscription,
      ClientTraceData ctData)
    {
      if (!string.Equals(subscription.EventTypeName, "WorkItemChangedEvent", StringComparison.OrdinalIgnoreCase))
        return;
      ctData.Add(NotificationAuditing.SubscriptionExpressionProperty, (object) subscription.Expression);
    }
  }
}
