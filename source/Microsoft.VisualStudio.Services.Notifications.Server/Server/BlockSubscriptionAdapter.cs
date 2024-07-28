// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.BlockSubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class BlockSubscriptionAdapter : RoleBasedSubscriptionAdapter
  {
    public override string FilterType => "Block";

    protected override ISubscriptionFilter CreateInnerSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      ExpressionFilterModel filterModel = queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeFilterDetails) ? this.InnerPathAdapter.ParseCondition(requestContext, subscription.ConditionString) : new ExpressionFilterModel();
      return (ISubscriptionFilter) new BlockFilter(subscription.EventTypeName, filterModel);
    }

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscription notificationSubscription)
    {
      Subscription subscription = base.ToSubscription(requestContext, notificationSubscription);
      subscription.Channel = "Block";
      subscription.Matcher = "BlockMatcher";
      return subscription;
    }

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters)
    {
      Subscription subscription = base.ToSubscription(requestContext, createParameters);
      subscription.Channel = "Block";
      subscription.Matcher = "BlockMatcher";
      return subscription;
    }

    protected override string Matcher => "BlockMatcher";

    protected override void ValidateRoleBasedExpression(RoleBasedExpression expression)
    {
    }
  }
}
