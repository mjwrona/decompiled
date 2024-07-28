// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.LegacyMatcherFilterBase
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class LegacyMatcherFilterBase : BaseMatcherFilter
  {
    private const string s_area = "Notifications";
    private const string s_layer = "LegacyMatcherFilterBase";

    public override NotificationSubscription PrepareForDisplay(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      string artifactType = subscription.SubscriptionFilter is ArtifactFilter subscriptionFilter ? subscriptionFilter.ArtifactType : (string) null;
      bool throwIfEventTypeNull = !queryFlags.HasFlag((Enum) SubscriptionQueryFlags.AlwaysReturnBasicInformation) && !queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeInvalidSubscriptions);
      return SubscriptionAdapterFactory.CreateAdapter(requestContext, subscription.SubscriptionFilter?.EventType, subscription.Matcher, SubscriptionScope.Default, artifactType, throwIfEventTypeNull: throwIfEventTypeNull).ToNotificationSubscription(requestContext, subscription, queryFlags);
    }

    public override void PrepareForDisplay(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter)
    {
      SubscriptionAdapterFactory.CreateAdapter(requestContext, filter, new SubscriptionScope()).PrepareForDisplay(requestContext, filter);
    }

    public override Subscription PrepareForCreate(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters,
      bool isUserSubscription = true)
    {
      if (isUserSubscription)
        this.ValidateCustomSubscriptionCreateParameters(requestContext, createParameters);
      ISubscriptionAdapter adapter = SubscriptionAdapterFactory.CreateAdapter(requestContext, createParameters.Filter, createParameters.Scope);
      Subscription subscription = adapter.ToSubscription(requestContext, createParameters);
      SubscriptionQueryFlags queryFlags = isUserSubscription ? SubscriptionQueryFlags.IncludeFilterDetails : SubscriptionQueryFlags.None;
      subscription.ToLegacyEventType(requestContext);
      this.ReplaceEmptyIdentitiesWithDefaults(requestContext, subscription);
      this.UpdateIdentityFields(requestContext, subscription);
      this.SetSubscriptionFlags(requestContext, subscription);
      this.PreBindFilter(requestContext, subscription, queryFlags);
      adapter.PreBindSubscription(requestContext, subscription);
      return subscription;
    }

    public override SubscriptionUpdate PrepareForUpdate(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      if (true)
        this.ValidateCustomSubscriptionUpdateParameters(requestContext, subscriptionToPatch, updateParameters);
      SubscriptionUpdate subscriptionUpdate = new SubscriptionUpdate(subscriptionToPatch.ID)
      {
        Description = updateParameters.Description,
        AdminSettings = updateParameters.AdminSettings,
        UserSettings = updateParameters.UserSettings,
        Channel = updateParameters.Channel != null ? updateParameters.Channel.Type : (string) null,
        Address = updateParameters.Channel != null ? SubscriptionChannel.GetAddress(updateParameters.Channel) : (string) null
      };
      this.ApplyStatusUpdates(updateParameters, subscriptionUpdate);
      if (updateParameters.Filter != null)
      {
        Subscription subscription = (Subscription) subscriptionToPatch.Clone();
        ISubscriptionAdapter adapter = SubscriptionAdapterFactory.CreateAdapter(requestContext, updateParameters.Filter, updateParameters.Scope);
        adapter.ApplyFilterUpdatesToSubscription(requestContext, subscription, updateParameters);
        this.PreBindFilter(requestContext, subscription, SubscriptionQueryFlags.IncludeFilterDetails);
        adapter.PreBindSubscription(requestContext, subscription);
        subscriptionUpdate.EventTypeName = EventTypeMapper.ToLegacy(requestContext, subscription.SubscriptionFilter.EventType);
        subscriptionUpdate.Expression = subscription.Expression;
        subscriptionUpdate.Matcher = subscription.Matcher;
        subscriptionUpdate.ScopeId = subscription.SubscriptionScope?.Id;
      }
      base.PrepareForUpdate(requestContext, subscriptionToPatch, subscriptionUpdate);
      this.SetSubscriptionUpdateFlags(subscriptionToPatch, subscriptionUpdate);
      return subscriptionUpdate;
    }

    public override void PrepareForUpdate(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      SubscriptionUpdate subscriptionUpdate)
    {
      if (!string.IsNullOrEmpty(subscriptionUpdate.Expression))
      {
        Subscription subscription1 = (Subscription) subscriptionToPatch.Clone();
        subscription1.Expression = subscriptionUpdate.Expression;
        subscription1.ConditionString = subscriptionUpdate.Expression;
        Guid? nullable1 = subscriptionUpdate.ScopeId;
        if (nullable1.HasValue)
        {
          Subscription subscription2 = subscription1;
          SubscriptionScope subscriptionScope = new SubscriptionScope();
          nullable1 = subscriptionUpdate.ScopeId;
          subscriptionScope.Id = nullable1.Value;
          subscription2.SubscriptionScope = subscriptionScope;
        }
        this.PreBindFilter(requestContext, subscription1, SubscriptionQueryFlags.IncludeFilterDetails);
        MatcherFilterFactory.GetMatcherFilter(requestContext, subscription1.Matcher).PreBindSubscription(requestContext, subscription1);
        subscriptionUpdate.Expression = subscription1.Expression;
        SubscriptionUpdate subscriptionUpdate1 = subscriptionUpdate;
        SubscriptionScope subscriptionScope1 = subscription1.SubscriptionScope;
        Guid? nullable2;
        if (subscriptionScope1 == null)
        {
          nullable1 = new Guid?();
          nullable2 = nullable1;
        }
        else
          nullable2 = new Guid?(subscriptionScope1.Id);
        subscriptionUpdate1.ScopeId = nullable2;
      }
      base.PrepareForUpdate(requestContext, subscriptionToPatch, subscriptionUpdate);
    }

    public override SubscriptionLookup ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionQueryCondition condition)
    {
      SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup();
      ISubscriptionFilter filter = condition.Filter;
      if (filter != null)
      {
        if (filter is UnsupportedFilter)
          throw new ArgumentException(CoreRes.InvalidSubscriptionFilter());
        ISubscriptionAdapter adapter = SubscriptionAdapterFactory.CreateAdapter(requestContext, condition.Filter, SubscriptionScope.Default, false);
        if (adapter != null)
          adapter.ApplyToSubscriptionLookup(requestContext, anyFieldLookup, condition.Filter);
        else if (!string.IsNullOrEmpty(filter.EventType))
          anyFieldLookup.EventType = filter.EventType;
      }
      return anyFieldLookup;
    }

    public override void PostBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      ISubscriptionAdapter defaultAdapter = subscription.GetDefaultAdapter(requestContext);
      defaultAdapter.PostBindSubscription(requestContext, subscription);
      base.PostBindSubscription(requestContext, subscription, queryFlags);
      if (subscription.SubscriptionFilter != null)
        return;
      subscription.SubscriptionFilter = defaultAdapter.CreateSubscriptionFilter(requestContext, subscription, queryFlags);
    }

    public override void PreBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      base.PreBindSubscription(requestContext, subscription, queryFlags);
      subscription.GetDefaultAdapter(requestContext).PreBindSubscription(requestContext, subscription);
    }

    protected void ReplaceProjectNamesWithGuids(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      INotificationBridge cachedExtension = requestContext.GetCachedExtension<INotificationBridge>("@NotifBridge");
      if (cachedExtension != null && !string.IsNullOrEmpty(subscription.ConditionString))
      {
        Guid guid = cachedExtension.ReplaceProjectNamesWithGuids(requestContext, subscription);
        if (subscription.ProjectId == Guid.Empty || subscription.ProjectId == NotificationClientConstants.CollectionScope)
          subscription.ProjectId = guid;
      }
      if (!(subscription.ProjectId != Guid.Empty) || cachedExtension == null)
        return;
      cachedExtension.CheckProjectPermissions(requestContext, subscription.ProjectId);
    }

    protected override void PostBindFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      this.ReplaceProjectGuidsWithNames(requestContext, subscription);
      this.ReplaceIdentityGuidsWithNames(requestContext, subscription);
      LegacyMatcherFilterBase.SetHasPeopleMacros(subscription);
    }

    protected override void PreBindFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      if (queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeFilterDetails))
      {
        this.ReplaceProjectNamesWithGuids(requestContext, subscription);
        this.ReplaceIdentityNamesWithGuids(requestContext, subscription);
      }
      LegacyMatcherFilterBase.SetHasPeopleMacros(subscription);
    }

    private static void SetHasPeopleMacros(Subscription subscription) => subscription.HasPeopleMacros = !string.IsNullOrEmpty(subscription.Expression) && (subscription.Expression.Contains("@@MyUniqueName@@") || subscription.Expression.Contains("@@MyDisplayName@@"));

    protected void ReplaceProjectGuidsWithNames(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      try
      {
        requestContext.GetCachedExtension<INotificationBridge>("@NotifBridge")?.ReplaceProjectGuidsWithNames(requestContext, subscription);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002021, "Notifications", nameof (LegacyMatcherFilterBase), ex);
      }
    }

    protected void ReplaceIdentityGuidsWithNames(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      foreach (ISubscriptionPersistence persistenceExtension in (IEnumerable<ISubscriptionPersistence>) this.GetPersistenceExtensions(requestContext))
      {
        try
        {
          if (persistenceExtension.IsSupportedEventType(subscription.EventTypeName))
            (ExtensionsUtil.CreateNewInstance((object) persistenceExtension) as ISubscriptionPersistence).AfterReadSubscription(requestContext, subscription);
        }
        catch (Exception ex)
        {
          requestContext.Trace(1002020, TraceLevel.Warning, "Notifications", nameof (LegacyMatcherFilterBase), subscription.TraceTags, "AfterReadSubscription: Error occurred while processing subscription filtering. Subscription ID {0}. Subscriber ID {1}. Event type {2}. Condition String {3} Exception {4}.", (object) subscription.ID, (object) subscription.SubscriberId, (object) subscription.EventTypeName, (object) subscription.ConditionString, (object) ex);
        }
      }
    }

    protected void ReplaceIdentityNamesWithGuids(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      foreach (ISubscriptionPersistence persistenceExtension in (IEnumerable<ISubscriptionPersistence>) this.GetPersistenceExtensions(requestContext))
      {
        try
        {
          if (persistenceExtension.IsSupportedEventType(subscription.SubscriptionFilter.EventType))
            (ExtensionsUtil.CreateNewInstance((object) persistenceExtension) as ISubscriptionPersistence).BeforeWriteSubscription(requestContext, subscription);
        }
        catch (Exception ex)
        {
          requestContext.Trace(1002020, TraceLevel.Warning, "Notifications", nameof (LegacyMatcherFilterBase), subscription.TraceTags, "BeforeWriteSubscription: Error occurred while processing subscription filtering - Exception {0}", (object) ex);
        }
      }
    }

    private IReadOnlyList<ISubscriptionPersistence> GetPersistenceExtensions(
      IVssRequestContext requestContext)
    {
      return requestContext.GetStaticNotificationExtentions<ISubscriptionPersistence>();
    }

    protected override void ValidateSubscriptionCondition(
      IVssRequestContext requestContext,
      Subscription subscription,
      bool skipWarning,
      int recipientsCount)
    {
      if (subscription.ConditionString == null)
        return;
      EvaluationContext evaluationContext = new EvaluationContext()
      {
        Verify = true,
        RegexTimeout = TimeSpan.FromSeconds(5.0),
        UseRegexMatch = requestContext.IsFeatureEnabled("NotificationJob.UseRegexForMatch"),
        Tracer = (ISubscriptionObjectTrace) new SubscriptionObjectTracer(requestContext, nameof (LegacyMatcherFilterBase)),
        Subscription = subscription,
        User = subscription.SubscriberIdentity,
        Event = (TeamFoundationEvent) new TeamFoundationVerifySubscriptionEvent()
      };
      try
      {
        using (IDisposableReadOnlyList<IDynamicEventPredicate> extensions = requestContext.GetExtensions<IDynamicEventPredicate>())
        {
          Evaluation.EvaluateCondition(requestContext, evaluationContext, (IReadOnlyList<IDynamicEventPredicate>) extensions);
          if (skipWarning || subscription.SubsStats.GetEvaluationTimeTaken() <= 120000 && recipientsCount <= 100)
            return;
          subscription.Warning = CoreRes.SubscriptionWarning();
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1002020, TraceLevel.Warning, "Notifications", nameof (LegacyMatcherFilterBase), subscription.TraceTags, "Error validating a request for a new subscription. It will not get commited. Exception: {0}", (object) ex);
        throw;
      }
    }
  }
}
