// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.INotificationSubscriptionServiceInternal
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [DefaultServiceImplementation(typeof (NotificationSubscriptionService))]
  internal interface INotificationSubscriptionServiceInternal : 
    INotificationSubscriptionService,
    IVssFrameworkService
  {
    IEnumerable<Subscription> GetContributedSubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionTargets,
      IEnumerable<string> allowedFilterTypes = null,
      bool forDisplay = true);

    void UpdateSubscriptionStatus(
      IVssRequestContext requestContext,
      Subscription originalSubscription,
      SubscriptionStatus newStatus,
      string newStatusMessage = null,
      bool suspendPendingNotificationsOnDisable = true);

    void UpdateSubscriptionDiagnostics(
      IVssRequestContext requestCOntext,
      IEnumerable<Subscription> subscriptions);

    void JailSubscription(
      IVssRequestContext requestContext,
      NotificationStatistic stat,
      DateTime jailOnlyfNewerThanDate,
      int jailSubscriptionUserThreshold,
      int jailSubscriptionServiceHooksThreshold,
      bool auditOnly = false);

    void UpdateSubscriptionStatus(
      IVssRequestContext requestContext,
      int subscriptionId,
      SubscriptionStatus newStatus,
      string newStatusMessage = null,
      bool suspendPendingNotificationsOnDisable = true);

    Microsoft.VisualStudio.Services.Identity.Identity GetValidUsersGroup(
      IVssRequestContext requestContext);

    bool IsJobRegistered(IVssRequestContext requestContext, Guid jobId);
  }
}
