// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionExtensions
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class SubscriptionExtensions
  {
    private const string s_area = "Notifications";
    private const string s_layer = "SubscriptionExtensions";
    private const string s_skipCacheForVsidReads = "VisualStudio.Services.Identity.SkipCacheForVsidReads";

    public static bool IsOptOutable(this Subscription subscription)
    {
      bool flag = false;
      if (subscription.IsContributed && (subscription.Matcher == "ActorMatcher" || subscription.Matcher == "ActorExpressionMatcher"))
        flag = true;
      if (!flag && subscription.IsGroup && !subscription.HasAddress && subscription.IsEmailDelivery && NotificationFrameworkConstants.OptOutMatcherCandidates.Contains(subscription.Matcher))
        flag = true;
      return flag;
    }

    public static bool IsEnabled(this Subscription subscription) => subscription.EffectiveStatus() >= SubscriptionStatus.Enabled;

    public static SubscriptionStatus EffectiveStatus(this Subscription subscription)
    {
      SubscriptionStatus subscriptionStatus = subscription.Status;
      if (subscriptionStatus >= SubscriptionStatus.Enabled && subscription.IsOptOutable() && !subscription.AdminSettingsBlockUserOptOut && subscription.UserSettingsOptedOut)
        subscriptionStatus = SubscriptionStatus.Disabled;
      return subscriptionStatus;
    }

    public static bool PostBindForEvaluation(
      this Subscription subscription,
      IVssRequestContext requestContext,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.IncludeFilterDetails,
      IEnumerable<ISubscriptionValidator> subscriptionValidators = null,
      NotificationStopwatch postBindStopwatch = null,
      NotificationStopwatch pluginStopwatch = null)
    {
      using (new AutoStopwatch((Stopwatch) postBindStopwatch))
      {
        try
        {
          try
          {
            subscription.PostBindSubscription(requestContext, queryFlags);
          }
          catch (Exception ex)
          {
            InvalidRoleBasedExpressionException expressionException = ex as InvalidRoleBasedExpressionException;
            if ((subscription.Matcher.Equals("ActorMatcher") ? 1 : (subscription.Matcher.Equals("BlockMatcher") ? 1 : 0)) != 0 && expressionException == null && !(ex is IdentityNotFoundException))
              throw new InvalidRoleBasedExpressionException(CoreRes.InvalidRoleBasedExpression((object) subscription.Expression), ex);
            throw;
          }
          if (subscription.Matcher != "FollowsMatcher")
          {
            string user1 = !string.IsNullOrEmpty(subscription.SubscriberIdentity.DisplayName) ? subscription.SubscriberIdentity.DisplayName : subscription.SubscriberId.ToString();
            string user2 = subscription.LastModifiedByIdentity == null || string.IsNullOrEmpty(subscription.LastModifiedByIdentity.DisplayName) ? subscription.LastModifiedBy.ToString() : subscription.LastModifiedByIdentity.DisplayName;
            if (subscription.LastModifiedByIdentity != null && !subscription.LastModifiedByIdentity.IsActive && subscription.LastModifiedByWillBeUsedForAuth())
              TryReadIdentitySkippingCache(subscription.LastModifiedByIdentity, false);
            else if (!subscription.SubscriberIdentity.IsActive)
              TryReadIdentitySkippingCache(subscription.SubscriberIdentity, true);

            void TryReadIdentitySkippingCache(Microsoft.VisualStudio.Services.Identity.Identity identity, bool isOwner)
            {
              if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.SkipCacheForVsidReads"))
              {
                requestContext.Trace(1002323, TraceLevel.Error, "Notifications", nameof (SubscriptionExtensions), string.Format("Identity Inactive (first attempt): {0} {1} / {2}", isOwner ? (object) "Subscriber" : (object) "Modified By", (object) identity?.Id, (object) identity?.Descriptor?.ToString()));
                IdentityService service = requestContext.GetService<IdentityService>();
                try
                {
                  requestContext.Items[RequestContextItemsKeys.BypassIdentityCacheWhenReadingByVsid] = (object) true;
                  IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
                  {
                    identity.Id
                  }, QueryMembership.Direct, (IEnumerable<string>) null);
                  if (identityList.Count < 1 || identityList[0] == null || !identityList[0].IsActive)
                  {
                    if (identityList.Count < 1 || identityList[0] == null)
                      requestContext.Trace(1002323, TraceLevel.Error, "Notifications", nameof (SubscriptionExtensions), string.Format("Identity Inactive (second attempt): could not read {0} identity {1} from IdentityService", isOwner ? (object) "Subscriber" : (object) "Modified By", (object) identity?.Id));
                    else
                      requestContext.Trace(1002323, TraceLevel.Error, "Notifications", nameof (SubscriptionExtensions), string.Format("Identity Inactive (second attempt): {0} {1} / {2}", isOwner ? (object) "Subscriber" : (object) "Modified By", (object) identityList[0]?.Id, (object) identityList[0]?.Descriptor?.ToString()));
                    throw new IdentityInactiveException(user1, user2);
                  }
                }
                finally
                {
                  requestContext.Items.Remove(RequestContextItemsKeys.BypassIdentityCacheWhenReadingByVsid);
                }
              }
              else
              {
                requestContext.Trace(1002323, TraceLevel.Error, "Notifications", nameof (SubscriptionExtensions), string.Format("Identity Inactive: {0} {1} / {2}", isOwner ? (object) "Subscriber" : (object) "Modified By", (object) identity?.Id, (object) identity?.Descriptor?.ToString()));
                throw new IdentityInactiveException(user1, user2);
              }
            }
          }
          using (new AutoStopwatch((Stopwatch) pluginStopwatch))
          {
            if (subscriptionValidators != null)
            {
              foreach (ISubscriptionValidator subscriptionValidator in subscriptionValidators)
                subscriptionValidator.ValidateSubscription(requestContext, subscription);
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceExceptionMsg(1002323, "Notifications", nameof (SubscriptionExtensions), ex, string.Format("Subscription ID: {0}, channel: {1}", (object) subscription.ID, (object) subscription.Channel));
          subscription.UpdateDisabledStatusFromException(requestContext, ex);
        }
        return subscription.IsEnabled();
      }
    }

    public static bool ScopeMatches(this Subscription subscription, IEnumerable<EventScope> scopes) => subscription.ScopeId.Equals(Guid.Empty) || subscription.ScopeId.Equals(NotificationClientConstants.CollectionScope) || scopes.Where<EventScope>((Func<EventScope, bool>) (es => subscription.ScopeId.Equals(es.Id))).Any<EventScope>();

    public static bool IsCollectionScoped(this Subscription subscription) => subscription.ScopeId.Equals(Guid.Empty) || subscription.ScopeId == NotificationClientConstants.CollectionScope;

    public static void RemoveXmlInvalidCharacters(this Subscription subscription)
    {
      subscription.ConditionString = subscription.ConditionString.Replace('\a'.ToString(), "");
      subscription.ConditionString = subscription.ConditionString.Replace('\v'.ToString(), "");
    }

    public static bool HasValidEventType(
      this Subscription subscription,
      IVssRequestContext requestContext,
      Dictionary<string, NotificationEventType> eventTypes)
    {
      if (string.IsNullOrEmpty(subscription.EventTypeName))
        return subscription.Matcher.Equals("FollowsMatcher");
      string contributed = EventTypeMapper.ToContributed(requestContext, subscription.EventTypeName);
      return eventTypes != null && eventTypes.ContainsKey(contributed);
    }

    public static bool CandidateIsElligible(
      this Subscription subscription,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      ProcessedEvent pe)
    {
      bool flag = true;
      if (subscription.IsEmailDelivery && ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor))
      {
        if (pe != null)
          pe.AddClause(identity, false, "not service principal");
        flag = false;
      }
      return flag;
    }
  }
}
