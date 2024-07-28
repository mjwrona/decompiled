// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IMatcherFilter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public interface IMatcherFilter
  {
    string Matcher { get; }

    void PostBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags);

    void PreBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.IncludeFilterDetails);

    NotificationSubscription PrepareForDisplay(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags);

    void PrepareForDisplay(IVssRequestContext requestContext, ISubscriptionFilter filter);

    SubscriptionUpdate PrepareForUpdate(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      NotificationSubscriptionUpdateParameters updateParameters);

    void PrepareForUpdate(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      SubscriptionUpdate subscriptionUpdate);

    Subscription PrepareForCreate(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters subscription,
      bool validate = true);

    SubscriptionLookup ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionQueryCondition condition);

    void ValidateCustomNewSubscription(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter,
      SubscriptionScope scope,
      string channel);
  }
}
