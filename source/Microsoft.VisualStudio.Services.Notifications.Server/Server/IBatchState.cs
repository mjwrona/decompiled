// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IBatchState
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public interface IBatchState
  {
    bool EventHasUniqueTarget(MatchTarget target, Subscription subscription);

    bool AddUniqueTargetForEvent(MatchTarget target, Subscription subscription);

    void AddTargetForEvent(MatchTarget target, Subscription subscription);

    void FlushUniqueTargetsForEventType(string EventType);

    void GetSubscriptionsForMatcher(
      string matcher,
      out List<Subscription> subscriptions,
      out Dictionary<string, HashSet<Guid>> userOptOuts);

    void GetSubscriptionsForMatchers(
      IEnumerable<string> matchers,
      out List<Subscription> subscriptions,
      out Dictionary<string, HashSet<Guid>> userOptOuts);

    void UpdateSubscriptionStatus(Subscription subscription);

    void UpdateSubscriptionStatistics(IEnumerable<Subscription> subscriptions);

    bool PrepAndValidateSubscription(Subscription subscription);

    INotificationProcessingJob Job { get; }

    TeamFoundationEvent CurrentEvent { get; }

    IVssRequestContext RequestContext { get; }

    IdentityService IdentityService { get; }

    BatchStatus Status { get; }

    IBatchSettings Settings { get; }

    EvaluationContext CreateEvaluationContext();

    EvaluationContext CreateEvaluationContext(
      Subscription subscription,
      TeamFoundationEvent ev,
      Microsoft.VisualStudio.Services.Identity.Identity user);
  }
}
