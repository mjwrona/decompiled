// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Utils.EventServiceHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Utils
{
  public class EventServiceHelper
  {
    private const string c_subscriberType = "SubscriberType";
    private const string c_prSubscriberName = "GitPullRequestCodeReviewSubscriber";

    public static void PublishDecisionPoint(
      IVssRequestContext requestContext,
      CodeReviewEventNotification crEvent)
    {
      try
      {
        requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) crEvent);
      }
      catch (ActionDeniedBySubscriberException ex)
      {
        if (ex.PropertyCollection.FirstOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (prop => prop.Key.Contains("SubscriberType", StringComparison.OrdinalIgnoreCase))).Value is string source && source.Contains("GitPullRequestCodeReviewSubscriber", StringComparison.OrdinalIgnoreCase))
          throw new CodeReviewActionRejectedByPullRequestException(ex);
        throw new CodeReviewActionRejectedByPolicyException(ex);
      }
    }

    public static void PublishNotification(
      IVssRequestContext requestContext,
      CodeReviewEventNotification crEvent,
      string area,
      string layer)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      crEvent.Requester = userIdentity.ToCRIdentityRef(requestContext);
      if (EventServiceHelper.ShouldSendRealtimeNotification(crEvent))
      {
        EventServiceHelper.UpdateNotification(crEvent);
        Microsoft.VisualStudio.Services.CodeReview.Server.Common.EventServiceHelper.SendRealtimeEvent(requestContext, crEvent, area, layer);
      }
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) crEvent);
    }

    private static bool ShouldSendRealtimeNotification(CodeReviewEventNotification crEvent) => (!(crEvent is IterationNotification iterationNotification) || !iterationNotification.Iteration.IsUnpublished) && !(crEvent is ShareReviewNotification);

    private static void UpdateNotification(CodeReviewEventNotification crEvent)
    {
      if (!(crEvent is IterationNotification iterationNotification) || iterationNotification.Iteration.ChangeList == null || iterationNotification.Iteration.ChangeList.Count <= ServerConstants.maxChangeListForPushNotification)
        return;
      iterationNotification.Iteration = iterationNotification.Iteration.ShallowClone();
    }
  }
}
