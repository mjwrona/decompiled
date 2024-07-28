// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.INotificationSubscriptionService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [DefaultServiceImplementation(typeof (NotificationSubscriptionService))]
  public interface INotificationSubscriptionService : IVssFrameworkService
  {
    Subscription GetSubscription(
      IVssRequestContext requestContext,
      string subscriptionId,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None);

    List<Subscription> GetSubscriptions(
      IVssRequestContext requestContext,
      List<string> subscriptionId,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None);

    NotificationSubscription GetNotificationSubscription(
      IVssRequestContext requestContext,
      string subscriptionId,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None);

    List<NotificationSubscription> GetNotificationSubscriptions(
      IVssRequestContext requestContext,
      List<string> subscriptionIds,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None);

    Subscription GetSubscription(
      IVssRequestContext requestContext,
      int id,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None);

    List<NotificationSubscription> QuerySubscriptions(
      IVssRequestContext tfsRequestContext,
      SubscriptionQuery subscriptionQuery);

    List<Subscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      SubscriptionLookup subscriptionKey,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None,
      bool updateProjectGuid = true);

    List<Subscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<SubscriptionLookup> subscriptionKeys,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None,
      bool updateProjectGuid = true);

    void DeleteSubscription(IVssRequestContext tfsRequestContext, string subscriptionId);

    void DeleteSubscription(IVssRequestContext requestContext, int subscriptionId);

    NotificationSubscription CreateSubscription(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters);

    int CreateSubscription(IVssRequestContext requestContext, Subscription subscription);

    NotificationSubscription UpdateSubscription(
      IVssRequestContext tfsRequestContext,
      string subscriptionId,
      NotificationSubscriptionUpdateParameters updateParameters,
      bool suspendPendingNotificationsOnDisable = true);

    void UpdateSubscription(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      SubscriptionUpdate subscriptionUpdate,
      bool suspendPendingNotificationsOnDisable = true);

    SubscriptionUserSettings UpdateSubscriptionUserSettings(
      IVssRequestContext requestContext,
      string subscriptionId,
      Guid userId,
      SubscriptionUserSettings userSettings);

    SubscriptionDiagnostics UpdateSubscriptionDiagnostics(
      IVssRequestContext requestContext,
      string subscriptionId,
      UpdateSubscripitonDiagnosticsParameters updateParameters);

    IEnumerable<NotificationSubscriptionTemplate> GetSubscriptionTemplates(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      SubscriptionTemplateQueryFlags queryFlags = SubscriptionTemplateQueryFlags.IncludeUserAndGroup);

    bool HasPermissions(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity, int permission);

    bool CallerHasAdminPermissions(IVssRequestContext requestContext);

    bool CallerHasManageSubscriptionsPermission(
      IVssRequestContext requestContext,
      IdentityRef subscriber);

    HashSet<string> GetAllowedChannelsForCustomSubscriptions(IVssRequestContext requestContext);

    IEnumerable<NotificationSubscription> GetNotificationSubscriptionsForTarget(
      IVssRequestContext requestContext,
      Guid targetId,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None);

    void UpdateSubscriptionProject(
      IVssRequestContext requestContext,
      Subscription subscription,
      Guid projectId);
  }
}
