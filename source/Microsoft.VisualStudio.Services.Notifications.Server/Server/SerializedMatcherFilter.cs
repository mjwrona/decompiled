// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SerializedMatcherFilter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class SerializedMatcherFilter : BaseMatcherFilter
  {
    private const string s_area = "Notifications";
    private const string s_layer = "SerializedMatcherFilterBase";

    public override string Matcher { get; }

    public override void PostBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      if (subscription.SubscriptionFilter == null && subscription.Expression != null)
        subscription.SubscriptionFilter = JsonConvert.DeserializeObject<ISubscriptionFilter>(subscription.Expression, NotificationsSerialization.SubscriptionFilterJsonSerializerSettings);
      if (subscription.SubscriptionFilter == null)
        throw new MatcherNotSupportedException("subscription " + subscription.SubscriptionId + " cannot be used with SerializedMatcherFilter");
      base.PostBindSubscription(requestContext, subscription, queryFlags);
    }

    public override NotificationSubscription PrepareForDisplay(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      subscription.ToContributedEventType(requestContext);
      NotificationSubscription notificationSubscription1 = new NotificationSubscription();
      notificationSubscription1.Id = subscription.SubscriptionId;
      Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity = subscription.SubscriberIdentity;
      notificationSubscription1.Subscriber = subscriberIdentity != null ? subscriberIdentity.ToIdentityRef() : (IdentityRef) null;
      Microsoft.VisualStudio.Services.Identity.Identity modifiedByIdentity = subscription.LastModifiedByIdentity;
      notificationSubscription1.LastModifiedBy = modifiedByIdentity != null ? modifiedByIdentity.ToIdentityRef() : (IdentityRef) null;
      notificationSubscription1.Channel = SubscriptionChannel.Create(subscription.Channel, subscription.DeliveryAddress);
      notificationSubscription1.Scope = subscription.SubscriptionScope;
      notificationSubscription1.ModifiedDate = subscription.ModifiedTime;
      notificationSubscription1.Description = subscription.Description;
      notificationSubscription1.Status = subscription.Status;
      notificationSubscription1.StatusMessage = subscription.StatusMessage;
      notificationSubscription1.Flags = subscription.Flags;
      notificationSubscription1.Permissions = subscription.Permissions;
      notificationSubscription1.Filter = subscription.SubscriptionFilter;
      notificationSubscription1.UniqueId = subscription.UniqueId.ToString("D");
      NotificationSubscription notificationSubscription2 = notificationSubscription1;
      if (subscription.IsOptOutable())
      {
        notificationSubscription2.UserSettings = new SubscriptionUserSettings()
        {
          OptedOut = subscription.UserSettingsOptedOut
        };
        notificationSubscription2.AdminSettings = new SubscriptionAdminSettings()
        {
          BlockUserOptOut = subscription.AdminSettingsBlockUserOptOut
        };
      }
      notificationSubscription2.Url = NotificationSubscriptionService.GetSubscriptionRestURL(requestContext, subscription.SubscriptionId);
      if (!string.IsNullOrEmpty(subscription.DeliveryAddress) && (subscription.IsEmailDelivery || subscription.IsSoapDelivery))
        ((SubscriptionChannelWithAddress) notificationSubscription2.Channel).UseCustomAddress = true;
      this.PopulateNotificationSubscriptionLinks(requestContext, notificationSubscription2.Links, subscription);
      return notificationSubscription2;
    }

    public override void PrepareForDisplay(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter)
    {
    }

    public override Subscription PrepareForCreate(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters,
      bool isUserSubscription = true)
    {
      if (isUserSubscription)
        this.ValidateCustomSubscriptionCreateParameters(requestContext, createParameters);
      Guid empty = Guid.Empty;
      string deliveryPrefernce = this.ValidateAndParseDeliveryPrefernce(createParameters);
      Microsoft.VisualStudio.Services.Identity.Identity identity = createParameters.Subscriber == null ? requestContext.GetUserIdentity() : createParameters.Subscriber.ToVsIdentity(requestContext);
      Guid id = identity.Id;
      Subscription subscription = new Subscription()
      {
        Matcher = this.Matcher,
        Description = createParameters.Description,
        SubscriberIdentity = identity,
        SubscriberId = id,
        Channel = createParameters.Channel?.Type,
        DeliveryAddress = deliveryPrefernce,
        SubscriptionScope = createParameters.Scope ?? SubscriptionScope.Default,
        SubscriptionFilter = createParameters.Filter
      };
      if (isUserSubscription)
      {
        int recipientsCount = this.ValidateDeliveryPreferences(requestContext, subscription.SubscriberIdentity, subscription.Channel, subscription.DeliveryAddress);
        this.ValidateSubscriptionCondition(requestContext, subscription, false, recipientsCount);
      }
      SubscriptionQueryFlags queryFlags = isUserSubscription ? SubscriptionQueryFlags.IncludeFilterDetails : SubscriptionQueryFlags.None;
      this.PreBindSubscription(requestContext, subscription, queryFlags);
      subscription.Expression = JsonConvert.SerializeObject((object) subscription.SubscriptionFilter, NotificationsSerialization.SubscriptionFilterJsonSerializerSettings);
      return subscription;
    }

    protected override void PostBindFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      ExpressionFilter subscriptionFilter = subscription.SubscriptionFilter as ExpressionFilter;
      this.PostBindExpressionFilterModel(requestContext, subscription, subscriptionFilter.FilterModel, queryFlags);
    }

    protected override void PreBindFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      ExpressionFilter subscriptionFilter = subscription.SubscriptionFilter as ExpressionFilter;
      this.PreBindExpressionFilterModel(requestContext, subscription, subscriptionFilter.FilterModel, queryFlags);
    }

    protected void PostBindExpressionFilterModel(
      IVssRequestContext requestContext,
      Subscription subscription,
      ExpressionFilterModel model,
      SubscriptionQueryFlags queryFlags)
    {
      INotificationBridge cachedExtension = requestContext.GetCachedExtension<INotificationBridge>("@NotifBridge");
      IdentityService service = requestContext.GetService<IdentityService>();
      SubscriptionFieldProvider fieldProvider = subscription.GetFieldProvider(requestContext);
      Dictionary<Guid, string> identityGuidMap = new Dictionary<Guid, string>();
      bool flag = queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeFilterDetails);
      foreach (ExpressionFilterClause clause in (IEnumerable<ExpressionFilterClause>) model.Clauses)
      {
        NotificationEventField fieldByName = fieldProvider.GetFieldByName(clause.FieldName, false);
        if (fieldByName != null)
        {
          if (flag)
          {
            NotificationEventFieldType fieldType = fieldByName.FieldType;
            if ((fieldType != null ? (fieldType.SubscriptionFieldType == SubscriptionFieldType.TeamProject ? 1 : 0) : 0) != 0)
            {
              SerializedMatcherFilter.HandleTeamProjectIdClause(requestContext, cachedExtension, clause);
              continue;
            }
          }
          NotificationEventFieldType fieldType1 = fieldByName.FieldType;
          if ((fieldType1 != null ? (fieldType1.SubscriptionFieldType == SubscriptionFieldType.Identity ? 1 : 0) : 0) != 0)
          {
            if (flag)
              SerializedMatcherFilter.HandleIdentityIdClause(requestContext, service, identityGuidMap, clause);
            if (!subscription.HasPeopleMacros && (clause.Value.Equals("@@MyUniqueName@@", StringComparison.OrdinalIgnoreCase) || clause.Value.Equals("@@MyDisplayName@@", StringComparison.OrdinalIgnoreCase)))
              subscription.HasPeopleMacros = true;
          }
        }
      }
    }

    protected void PreBindExpressionFilterModel(
      IVssRequestContext requestContext,
      Subscription subscription,
      ExpressionFilterModel model,
      SubscriptionQueryFlags queryFlags)
    {
      INotificationBridge cachedExtension = requestContext.GetCachedExtension<INotificationBridge>("@NotifBridge");
      IdentityService service = requestContext.GetService<IdentityService>();
      SubscriptionFieldProvider fieldProvider = subscription.GetFieldProvider(requestContext);
      Dictionary<string, Guid> displayNameMap = new Dictionary<string, Guid>();
      bool flag = queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeFilterDetails);
      foreach (ExpressionFilterClause clause in (IEnumerable<ExpressionFilterClause>) model.Clauses)
      {
        NotificationEventField fieldByName = fieldProvider.GetFieldByName(clause.FieldName, false);
        if (fieldByName != null)
        {
          if (flag)
          {
            NotificationEventFieldType fieldType = fieldByName.FieldType;
            if ((fieldType != null ? (fieldType.SubscriptionFieldType == SubscriptionFieldType.TeamProject ? 1 : 0) : 0) != 0)
            {
              SerializedMatcherFilter.HandleTeamProjectNameClause(requestContext, cachedExtension, clause);
              continue;
            }
          }
          NotificationEventFieldType fieldType1 = fieldByName.FieldType;
          if ((fieldType1 != null ? (fieldType1.SubscriptionFieldType == SubscriptionFieldType.Identity ? 1 : 0) : 0) != 0)
          {
            if (flag)
              SerializedMatcherFilter.HandleIdentityNameClause(requestContext, service, displayNameMap, clause);
            if (!subscription.HasPeopleMacros && (clause.Value.Equals("@@MyUniqueName@@", StringComparison.OrdinalIgnoreCase) || clause.Value.Equals("@@MyDisplayName@@", StringComparison.OrdinalIgnoreCase)))
              subscription.HasPeopleMacros = true;
          }
        }
      }
    }

    private static void HandleIdentityIdClause(
      IVssRequestContext requestContext,
      IdentityService identityService,
      Dictionary<Guid, string> identityGuidMap,
      ExpressionFilterClause clause)
    {
      try
      {
        string str = (string) null;
        Guid result;
        if (Guid.TryParse(clause.Value, out result) && !identityGuidMap.TryGetValue(result, out str))
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = identityService.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
          {
            result
          }, QueryMembership.None, (IEnumerable<string>) null);
          if (identityList.Count > 0 && identityList[0] != null)
            str = identityList[0].CustomDisplayName;
        }
        if (str == null)
          return;
        clause.Value = str;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002033, "Notifications", "SerializedMatcherFilterBase", ex);
      }
    }

    private static void HandleTeamProjectIdClause(
      IVssRequestContext requestContext,
      INotificationBridge bridge,
      ExpressionFilterClause clause)
    {
      try
      {
        Guid result;
        if (!Guid.TryParse(clause.Value, out result))
          return;
        string projectName = bridge.GetProjectName(requestContext, result);
        if (string.IsNullOrEmpty(projectName))
          return;
        clause.Value = projectName;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002034, "Notifications", "SerializedMatcherFilterBase", ex);
      }
    }

    private static void HandleIdentityNameClause(
      IVssRequestContext requestContext,
      IdentityService identityService,
      Dictionary<string, Guid> displayNameMap,
      ExpressionFilterClause clause)
    {
      Guid guid = Guid.Empty;
      string str = clause.Value;
      if (string.IsNullOrEmpty(str) || str.Equals("@@MyDisplayName@@", StringComparison.OrdinalIgnoreCase) || str.Equals("@@MyUniqueName@@", StringComparison.OrdinalIgnoreCase))
        return;
      if (displayNameMap.TryGetValue(str, out guid))
        return;
      try
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = identityService.ReadIdentities(requestContext, IdentitySearchFilter.DisplayName, str, QueryMembership.None, (IEnumerable<string>) null);
        if (identityList.Count > 0 && identityList[0] != null)
          guid = identityList[0].Id;
        if (!(guid != Guid.Empty))
          return;
        clause.Value = guid.ToString();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002035, "Notifications", "SerializedMatcherFilterBase", ex);
      }
    }

    private static void HandleTeamProjectNameClause(
      IVssRequestContext requestContext,
      INotificationBridge bridge,
      ExpressionFilterClause clause)
    {
      try
      {
        Guid projectId = bridge.GetProjectId(requestContext, clause.Value);
        if (!(projectId != Guid.Empty))
          return;
        clause.Value = projectId.ToString();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002036, "Notifications", "SerializedMatcherFilterBase", ex);
      }
    }

    public override SubscriptionLookup ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionQueryCondition condition)
    {
      ArgumentUtility.CheckForNull<SubscriptionQueryCondition>(condition, nameof (condition));
      ArgumentUtility.CheckForNull<ISubscriptionFilter>(condition.Filter, "filter");
      return SubscriptionLookup.CreateEventTypeLookup(condition.Filter.EventType, this.Matcher);
    }

    public override SubscriptionUpdate PrepareForUpdate(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      this.ValidateCustomSubscriptionUpdateParameters(requestContext, originalSubscription, updateParameters);
      SubscriptionUpdate subscriptionUpdate = new SubscriptionUpdate(originalSubscription.SubscriptionId)
      {
        Description = updateParameters.Description,
        AdminSettings = updateParameters.AdminSettings,
        UserSettings = updateParameters.UserSettings
      };
      if (updateParameters.Filter != null)
      {
        if (!string.IsNullOrEmpty(updateParameters.Filter.EventType))
        {
          updateParameters.Filter.EventType = EventTypeMapper.ToLegacy(requestContext, updateParameters.Filter.EventType);
          subscriptionUpdate.EventTypeName = updateParameters.Filter.EventType;
        }
        subscriptionUpdate.Matcher = this.Matcher;
      }
      if (updateParameters.Scope?.Id.HasValue)
        subscriptionUpdate.ScopeId = new Guid?(updateParameters.Scope.Id);
      if (updateParameters.Filter != null)
        subscriptionUpdate.Expression = JsonConvert.SerializeObject((object) updateParameters.Filter, NotificationsSerialization.SubscriptionFilterJsonSerializerSettings);
      this.SetSubscriptionUpdateFlags(originalSubscription, subscriptionUpdate);
      this.ApplyStatusUpdates(updateParameters, subscriptionUpdate);
      return subscriptionUpdate;
    }

    protected string ValidateAndParseDeliveryPrefernce(
      NotificationSubscriptionCreateParameters notificationSubscription)
    {
      return SubscriptionChannel.GetAddress(notificationSubscription.Channel);
    }

    protected void PopulateNotificationSubscriptionLinks(
      IVssRequestContext requestContext,
      ReferenceLinks referenceLinks,
      Subscription subscription)
    {
      if (subscription.SubscriberIdentity.IsContainer && !subscription.Flags.HasFlag((Enum) SubscriptionFlags.ContributedSubscription))
      {
        string teamSubscriptionUrl = this.GetTeamSubscriptionUrl(requestContext, subscription);
        if (string.IsNullOrEmpty(teamSubscriptionUrl))
          return;
        referenceLinks.AddLink("edit", teamSubscriptionUrl);
        referenceLinks.AddLink("team", teamSubscriptionUrl);
      }
      else
      {
        string viewSubscriptionUrl = this.GetViewSubscriptionUrl(requestContext, subscription);
        if (string.IsNullOrEmpty(viewSubscriptionUrl))
          return;
        referenceLinks.AddLink("edit", viewSubscriptionUrl);
      }
    }

    protected string GetTeamSubscriptionUrl(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      try
      {
        if (requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
        {
          string str1 = subscription.ScopeId.Equals(NotificationClientConstants.CollectionScope) || Guid.Empty.Equals(subscription.ScopeId) ? string.Empty : subscription.ScopeId.ToString();
          Guid projectId = subscription.ProjectId;
          string str2 = !Guid.Empty.Equals(subscription.ProjectId) ? subscription.ProjectId.ToString() : str1;
          string str3 = requestContext.GetService<IContributionRoutingService>().RouteUrl(requestContext, "ms.vss-notifications-web.team-notifications-route", new RouteValueDictionary()
          {
            {
              "project",
              (object) str2
            },
            {
              "team",
              (object) subscription.SubscriberId
            }
          });
          if (!string.IsNullOrEmpty(str3))
          {
            StringBuilder stringBuilder = new StringBuilder(str3);
            stringBuilder.Append("?");
            stringBuilder.AppendFormat("{0}={1}", (object) "subscriptionId", (object) subscription.SubscriptionId);
            NotificationEventType eventType = requestContext.GetService<INotificationEventService>().GetEventType(requestContext, subscription.SubscriptionFilter.EventType);
            if (eventType != null && eventType.EventPublisher != null)
              stringBuilder.AppendFormat("&{0}={1}", (object) "publisherId", (object) eventType.EventPublisher.Id);
            return stringBuilder.ToString();
          }
        }
      }
      catch (Exception ex)
      {
      }
      return string.Empty;
    }

    internal virtual string GetViewSubscriptionUrl(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      string notificationsUrl = NotificationSubscriptionService.GetMyNotificationsUrl(requestContext, subscription);
      return !string.IsNullOrEmpty(notificationsUrl) ? new Uri(notificationsUrl).AppendQuery("action", "view").ToString() : string.Empty;
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
        Tracer = (ISubscriptionObjectTrace) new SubscriptionObjectTracer(requestContext, "SerializedMatcherFilterBase"),
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
        requestContext.Trace(1002020, TraceLevel.Warning, "Notifications", "SerializedMatcherFilterBase", subscription.TraceTags, "Error validating a request for a new subscription. It will not get commited. Exception: {0}", (object) ex);
        throw;
      }
    }
  }
}
