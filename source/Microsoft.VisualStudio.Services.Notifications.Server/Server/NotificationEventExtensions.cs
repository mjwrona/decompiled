// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationEventExtensions
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class NotificationEventExtensions
  {
    internal static void NotifyIfEventCreatedDeltaExceeded(
      this SerializedNotificationEvent evt,
      IVssRequestContext requestContext,
      int eventCreatedDeltaTriggerMilliseconds)
    {
      DateTime utcNow = DateTime.UtcNow;
      double num = !evt.SourceEventCreatedTime.HasValue ? (utcNow - evt.VssNotificationEventCreatedTime).TotalMilliseconds : (utcNow - evt.SourceEventCreatedTime.Value).TotalMilliseconds;
      if (num <= (double) eventCreatedDeltaTriggerMilliseconds)
        return;
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("DelayLimit", (double) eventCreatedDeltaTriggerMilliseconds);
      ciData.Add("Delay", num);
      ciData.Add("Now", (object) utcNow);
      ciData.Add("SourceEventCreatedTime", (object) evt.SourceEventCreatedTime);
      ciData.Add("VssSourceEventCreatedTime", (object) evt.VssNotificationEventCreatedTime);
      ciData.Add("ItemId", evt.ItemId);
      NotificationCustomerIntelligence.PublishEvent(requestContext, NotificationCustomerIntelligence.NotificationEventServiceFeature, NotificationCustomerIntelligence.PublishDelayedEventAction, ciData);
    }

    internal static string SerializeEvent(this SerializedNotificationEvent evt) => JsonConvert.SerializeObject((object) evt, NotificationsSerialization.EmbedTypeJsonSerializerSettings);

    internal static string EventDataString(this VssNotificationEvent evt) => evt.Data as string;

    internal static Guid GetActor(this VssNotificationEvent evt, string role)
    {
      EventActor eventActor = evt.Actors.FirstOrDefault<EventActor>((Func<EventActor, bool>) (a => a.Role == role));
      return eventActor == null ? Guid.Empty : eventActor.Id;
    }

    internal static Guid GetInitiator(this VssNotificationEvent evt) => evt.GetActor(VssNotificationEvent.Roles.Initiator);

    internal static Guid GetMainActorId(this VssNotificationEvent evt) => evt.GetActor(VssNotificationEvent.Roles.MainActor);

    internal static Guid GetScope(this VssNotificationEvent evt, string type)
    {
      EventScope eventScope = evt.Scopes.FirstOrDefault<EventScope>((Func<EventScope, bool>) (s => s.Type == type));
      return eventScope == null ? Guid.Empty : eventScope.Id;
    }

    internal static Guid GetActor(this TeamFoundationEvent evt, string role)
    {
      EventActor eventActor = evt.Actors.FirstOrDefault<EventActor>((Func<EventActor, bool>) (a => a.Role == role));
      return eventActor == null ? Guid.Empty : eventActor.Id;
    }

    internal static Guid GetInitiator(this TeamFoundationEvent evt) => evt.GetActor(VssNotificationEvent.Roles.Initiator);

    internal static Guid GetScope(this TeamFoundationEvent evt, string type)
    {
      EventScope eventScope = evt.Scopes.FirstOrDefault<EventScope>((Func<EventScope, bool>) (s => s.Type == type));
      return eventScope == null ? Guid.Empty : eventScope.Id;
    }

    internal static string WhyIsSubscriptionIneligible(
      this TeamFoundationEvent evt,
      Subscription subscription)
    {
      if (evt.IsBlocked)
        return "event blocked";
      if (!subscription.IsEnabled())
        return "subscription disabled";
      if (!subscription.ScopeMatches((IEnumerable<EventScope>) evt.Scopes))
      {
        string str = string.Join<string>(",", evt.Scopes.Select<EventScope, string>((Func<EventScope, string>) (s => s.Id.ToString())));
        return string.Format("Scope {0} not in {1}", (object) subscription.ScopeId, (object) str);
      }
      return !evt.IsChannelEligible(subscription.Channel) ? subscription.Channel + " disallowed" : "ineligibility unknown";
    }

    internal static bool IsSubscriptionEligible(
      this TeamFoundationEvent evt,
      Subscription subscription,
      bool checkEventType = false)
    {
      return !evt.IsBlocked && subscription.IsEnabled() && subscription.ScopeMatches((IEnumerable<EventScope>) evt.Scopes) && (!checkEventType || evt.EventType.Equals(subscription.SubscriptionFilter.EventType)) && evt.IsChannelEligible(subscription.Channel);
    }

    internal static bool IsChannelEligible(this TeamFoundationEvent evt, string channel)
    {
      if (string.IsNullOrEmpty(channel))
        return false;
      return evt.AllowedChannels == null || evt.AllowedChannels.Contains(channel);
    }
  }
}
