// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.PathMatcherFilter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class PathMatcherFilter : SerializedMatcherFilter
  {
    public override string Matcher => "PathExpressionMatcher";

    public override SubscriptionLookup ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionQueryCondition condition)
    {
      SubscriptionLookup subscriptionLookup = base.ApplyToSubscriptionLookup(requestContext, condition);
      if ((condition.Filter as ExpressionFilter).FilterModel == null)
        return subscriptionLookup;
      throw new ArgumentException(NotificationsWebApiResources.Error_CannotSearchWithCriteria());
    }

    public override Subscription PrepareForCreate(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters notificationSubscription,
      bool validate = true)
    {
      Subscription subscription = base.PrepareForCreate(requestContext, notificationSubscription, true);
      if (notificationSubscription.Description != null)
        subscription.Tag = notificationSubscription.Description;
      return subscription;
    }

    public override SubscriptionUpdate PrepareForUpdate(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      SubscriptionUpdate subscriptionUpdate = base.PrepareForUpdate(requestContext, originalSubscription, updateParameters);
      if (updateParameters.Channel != null)
      {
        subscriptionUpdate.Channel = updateParameters.Channel.Type;
        subscriptionUpdate.Address = SubscriptionChannel.GetAddress(updateParameters.Channel);
      }
      return subscriptionUpdate;
    }
  }
}
