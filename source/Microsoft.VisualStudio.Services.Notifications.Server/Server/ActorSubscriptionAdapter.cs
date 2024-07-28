// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ActorSubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class ActorSubscriptionAdapter : RoleBasedSubscriptionAdapter
  {
    public override string FilterType => "Actor";

    protected override ISubscriptionFilter CreateInnerSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      ExpressionFilterModel filterModel = queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeFilterDetails) ? this.InnerPathAdapter.ParseCondition(requestContext, subscription.ConditionString) : new ExpressionFilterModel();
      return (ISubscriptionFilter) new ActorFilter(subscription.EventTypeName, filterModel);
    }

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscription notificationSubscription)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(notificationSubscription.Filter.EventType, "FilterEventType");
      Subscription subscription = base.ToSubscription(requestContext, notificationSubscription);
      if (string.IsNullOrEmpty(subscription.Channel))
        subscription.Channel = "User";
      subscription.Matcher = "ActorMatcher";
      return subscription;
    }

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(createParameters.Filter.EventType, "FilterEventType");
      Subscription subscription = base.ToSubscription(requestContext, createParameters);
      if (string.IsNullOrEmpty(subscription.Channel))
        subscription.Channel = "User";
      subscription.Matcher = "ActorMatcher";
      return subscription;
    }

    protected override string Matcher => "ActorMatcher";

    protected override void ValidateRoleBasedExpression(RoleBasedExpression expression)
    {
      if (expression.Inclusions.Count == 0)
        throw new ArgumentException(CoreRes.DefaultSubscriptionMustHaveInclusions(), "inclusions");
    }
  }
}
