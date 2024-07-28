// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ISubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [InheritedExport]
  public interface ISubscriptionAdapter
  {
    string SubscriptionTypeName { get; }

    string FilterType { get; }

    void InitializeAdapter(
      IVssRequestContext requestContext,
      string eventType,
      SubscriptionScope scopeId);

    EventSerializerType GetSerializationFormatForEvent(
      IVssRequestContext requestContext,
      string eventType);

    string GetMatcher(IVssRequestContext requestContext, string eventType);

    void PrepareForDisplay(IVssRequestContext requestContext, ISubscriptionFilter filter);

    NotificationSubscription ToNotificationSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags);

    void ApplyFilterUpdatesToSubscription(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      NotificationSubscriptionUpdateParameters updateParameters);

    Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscription subscription);

    Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters);

    void ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionLookup lookup,
      ISubscriptionFilter filter);

    void PostBindSubscription(IVssRequestContext requestContext, Subscription subscription);

    void PreBindSubscription(IVssRequestContext requestContext, Subscription subscription);

    Condition GetOptimizedCondition(Token fieldName, byte op, Token target);

    List<NotificationEventRole> GetRoles(IVssRequestContext requestContext);

    ISubscriptionFilter CreateSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags);

    bool AllowDuplicateSubscriptions();
  }
}
