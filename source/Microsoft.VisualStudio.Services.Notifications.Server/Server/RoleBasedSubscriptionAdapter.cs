// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.RoleBasedSubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class RoleBasedSubscriptionAdapter : BaseSubscriptionAdapter
  {
    private const string s_area = "Notifications";
    private const string s_layer = "RoleBasedSubscriptionsAdapter";

    public override string SubscriptionTypeName => "*";

    public override string GetMatcher(IVssRequestContext requestContext, string eventType) => this.Matcher;

    public override void InitializeAdapter(
      IVssRequestContext requestContext,
      string eventType,
      SubscriptionScope scopeId)
    {
      if (string.IsNullOrEmpty(eventType))
        return;
      string matcher = this.GetSerializationFormatForEvent(requestContext, eventType) == EventSerializerType.Json ? "JsonPathMatcher" : "XPathMatcher";
      this.InnerPathAdapter = SubscriptionAdapterFactory.CreateAdapter(requestContext, eventType, matcher, scopeId) as PathSubscriptionAdapter;
      ArgumentUtility.CheckForNull<PathSubscriptionAdapter>(this.InnerPathAdapter, "InnerPathAdapter");
      this.InnerPathAdapter.InitializeAdapter(requestContext);
    }

    protected abstract ISubscriptionFilter CreateInnerSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags);

    protected abstract string Matcher { get; }

    public override NotificationSubscription ToNotificationSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Subscription>(subscription, nameof (subscription));
      NotificationSubscription notificationSubscription = base.ToNotificationSubscription(requestContext, subscription, queryFlags);
      this.PopulateNotificationSubscriptionLinks(requestContext, subscription, notificationSubscription);
      return notificationSubscription;
    }

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscription notificationSubscription)
    {
      Subscription subscription = base.ToSubscription(requestContext, notificationSubscription);
      this.SetSubscriptionFilter(requestContext, subscription, notificationSubscription.Filter);
      return subscription;
    }

    public override void PrepareForDisplay(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter)
    {
      this.InnerPathAdapter.PrepareForDisplay(requestContext, filter);
    }

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters)
    {
      Subscription subscription = base.ToSubscription(requestContext, createParameters);
      this.SetSubscriptionFilter(requestContext, subscription, createParameters.Filter);
      return subscription;
    }

    private void SetSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      ISubscriptionFilter filter)
    {
      RoleBasedFilter roleBasedFilter = filter as RoleBasedFilter;
      ArgumentUtility.CheckForNull<RoleBasedFilter>(roleBasedFilter, "roleFilter");
      subscription.ConditionString = roleBasedFilter.FilterModel == null ? string.Empty : this.InnerPathAdapter.GetValidatedConditionString(requestContext, (ExpressionFilter) roleBasedFilter);
      RoleBasedExpression roleBasedExpression = new RoleBasedExpression()
      {
        Inclusions = new HashSet<string>((IEnumerable<string>) roleBasedFilter.Inclusions),
        Exclusions = new HashSet<string>((IEnumerable<string>) roleBasedFilter.Exclusions),
        Condition = subscription.ConditionString
      };
      roleBasedExpression.Validate();
      subscription.Expression = roleBasedExpression.Serialize();
    }

    public override void PostBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      if (subscription.Expression == null)
        return;
      RoleBasedExpression roleBasedExpression = RoleBasedExpression.Deserialize(subscription.Expression);
      subscription.ConditionString = roleBasedExpression.Condition;
    }

    public override ISubscriptionFilter CreateSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      RoleBasedFilter subscriptionFilter = this.CreateInnerSubscriptionFilter(requestContext, subscription, queryFlags) as RoleBasedFilter;
      if (subscription.Expression != null)
      {
        RoleBasedExpression roleBasedExpression = RoleBasedExpression.Deserialize(subscription.Expression);
        subscriptionFilter.Inclusions = (IList<string>) roleBasedExpression.Inclusions.ToList<string>();
        subscriptionFilter.Exclusions = (IList<string>) roleBasedExpression.Exclusions.ToList<string>();
      }
      else
      {
        requestContext.Trace(1002029, TraceLevel.Error, "Notifications", "RoleBasedSubscriptionsAdapter", "Null expression on subscription ID {0}, project ID {1}", (object) subscription.ID, (object) subscription.ScopeId);
        subscriptionFilter.Inclusions = (IList<string>) new List<string>();
        subscriptionFilter.Exclusions = (IList<string>) new List<string>();
      }
      return (ISubscriptionFilter) subscriptionFilter;
    }

    public override void PreBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      RoleBasedFilter subscriptionFilter = subscription.SubscriptionFilter as RoleBasedFilter;
      RoleBasedExpression roleBasedExpression = new RoleBasedExpression()
      {
        Inclusions = subscriptionFilter.Inclusions.ToHashSet(),
        Exclusions = subscriptionFilter.Exclusions.ToHashSet(),
        Condition = subscription.ConditionString
      };
      subscription.Expression = roleBasedExpression.Serialize();
    }

    protected abstract void ValidateRoleBasedExpression(RoleBasedExpression expression);

    public override void ApplyFilterUpdatesToSubscription(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      this.InnerPathAdapter.ApplyFilterUpdatesToSubscription(requestContext, subscriptionToPatch, updateParameters);
      if (!(updateParameters.Filter is RoleBasedFilter filter))
        return;
      subscriptionToPatch.SubscriptionFilter = (ISubscriptionFilter) filter;
      RoleBasedExpression roleBasedExpression = new RoleBasedExpression()
      {
        Inclusions = new HashSet<string>((IEnumerable<string>) filter.Inclusions),
        Exclusions = new HashSet<string>((IEnumerable<string>) filter.Exclusions),
        Condition = subscriptionToPatch.Expression
      };
      roleBasedExpression.Validate();
      subscriptionToPatch.Expression = roleBasedExpression.Serialize();
    }

    public override void ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionLookup lookup,
      ISubscriptionFilter filter)
    {
      ArgumentUtility.CheckForNull<SubscriptionLookup>(lookup, nameof (lookup));
      ArgumentUtility.CheckForNull<ISubscriptionFilter>(filter, nameof (filter));
      lookup.Matcher = this.Matcher;
      lookup.EventType = filter.EventType;
    }

    protected virtual void ValidateSubscription(Subscription subscription)
    {
    }

    public PathSubscriptionAdapter InnerPathAdapter { get; set; }
  }
}
