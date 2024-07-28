// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherSubscriptionsQueryController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "SubscriptionsQuery")]
  [ClientGroupByResource("subscriptions")]
  public class HooksPublisherSubscriptionsQueryController : ServiceHooksPublisherControllerBase
  {
    private static readonly string s_layer = typeof (HooksPublisherSubscriptionsQueryController).Name;
    private static readonly string s_area = typeof (HooksPublisherSubscriptionsQueryController).Namespace;

    [HttpPost]
    public SubscriptionsQuery CreateSubscriptionsQuery(SubscriptionsQuery query)
    {
      ArgumentUtility.CheckForNull<SubscriptionsQuery>(query, nameof (query));
      if (!string.IsNullOrEmpty(query.EventType))
        this.CheckScope(query.EventType);
      ServiceHooksTimer timer = ServiceHooksTimer.StartNew();
      bool flag1 = query.EventType != null;
      bool flag2 = query.PublisherId != null;
      bool flag3 = query.ConsumerId != null;
      bool flag4 = query.ConsumerActionId != null;
      bool flag5 = query.ConsumerInputFilters != null;
      bool flag6 = query.PublisherInputFilters != null;
      List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptionList = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
      List<string> userEventTypes = this.GetUserEventTypes();
      timer.RecordTick();
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) userEventTypes);
      IEnumerable<ServiceHooksPublisher> publishers = this.FindPublishers(query.PublisherId);
      timer.RecordTick();
      ServiceHooksTimer serviceHooksTimer = ServiceHooksTimer.StartNew();
      string str1 = "";
      foreach (ServiceHooksPublisher serviceHooksPublisher in publishers)
      {
        str1 = str1 + serviceHooksPublisher.Id + ",";
        IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions = serviceHooksPublisher.QuerySubscriptions(this.TfsRequestContext, query);
        serviceHooksTimer.RecordTick();
        foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in subscriptions)
        {
          if (stringSet.Contains(subscription.EventType))
            subscriptionList.Add(subscription);
        }
        serviceHooksTimer.RecordTick();
      }
      serviceHooksTimer.Stop();
      timer.RecordTick();
      query.Results = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptionList;
      query.Results.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      timer.RecordTick();
      if (!this.TfsRequestContext.IsFeatureEnabled("ServiceHooks.Subscriptions.SkipOverrideServiceIdentitiesStepForQuerySubscriptions"))
        query.Results.OverrideServiceIdentity(this.TfsRequestContext, timer);
      timer.Stop();
      string str2 = timer.Millis.ToString("D");
      string percents1 = timer.Percents;
      string str3 = serviceHooksTimer.Millis.ToString("D");
      string percents2 = serviceHooksTimer.Percents;
      this.TfsRequestContext.Trace(1063820, TraceLevel.Info, HooksPublisherSubscriptionsQueryController.s_area, HooksPublisherSubscriptionsQueryController.s_layer, "Overall time: " + str2 + ", overall percents: " + percents1 + ". Per-publisher time: " + str3 + ", per-publisher percents: " + percents2 + ". " + string.Format("Query HasEventType, {0}; HasPublisherId, {1}; SubscriberId, {2}; HasConsumerId, {3}; ", (object) flag1, (object) flag2, (object) query.SubscriberId, (object) flag3) + string.Format("HasConsumerActionId, {0}; HasConsumerInputFilters, {1}; HasPublisherInputFilters, {2}. ", (object) flag4, (object) flag5, (object) flag6) + string.Format("Result subscriptions count: {0}; Publishers: {1}", (object) subscriptionList.Count, (object) str1));
      return query;
    }
  }
}
