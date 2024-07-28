// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.NotificationUtil
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class NotificationUtil
  {
    public static VssNotificationEvent CreateNofiticationEvent(
      WikiPageChangeEvent changeEvent,
      bool pusherIsInitiator)
    {
      VssNotificationEvent nofiticationEvent = new VssNotificationEvent()
      {
        EventType = "ms.vss-wiki-web.wiki-page-changed-event",
        ItemId = changeEvent.PageId.ToString()
      };
      string artifactId = NotificationUtil.GetArtifactId(changeEvent);
      string artificatUri = LinkingUtilities.EncodeUri(new ArtifactId()
      {
        Tool = "Wiki",
        ArtifactType = "WikiPageId",
        ToolSpecificId = artifactId
      });
      nofiticationEvent.AddArtifactUri(artificatUri);
      if (pusherIsInitiator)
        nofiticationEvent.AddActor(VssNotificationEvent.Roles.Initiator, changeEvent.PusherId);
      nofiticationEvent.Data = (object) changeEvent;
      return nofiticationEvent;
    }

    public static string GetArtifactId(WikiPageChangeEvent changeEvent) => WikiPageIdHelper.GetArtifactId(changeEvent.ProjectId, changeEvent.WikiId, changeEvent.PageId);

    public static void SortCommitDetails(List<CommitDetails> commitDetails) => commitDetails?.Sort((Comparison<CommitDetails>) ((c1, c2) => c2.CommitTime.CompareTo(c1.CommitTime)));

    public static void FollowPage(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      int pageId,
      bool checkFollowStatusBeforeFollowing,
      string traceLayer,
      TimedCiEvent ciEvent)
    {
      try
      {
        bool flag = true;
        if (checkFollowStatusBeforeFollowing)
          flag = NotificationUtil.GetFollowSubscriptions(requestContext, projectId, wikiId, pageId, new Guid?(requestContext.GetUserIdentity().Id), ciEvent).Count == 0;
        if (!flag)
          return;
        using (new StopWatchHelper(ciEvent, nameof (FollowPage)))
        {
          NotificationSubscriptionCreateParameters createParameters = new NotificationSubscriptionCreateParameters()
          {
            Filter = (ISubscriptionFilter) new ArtifactFilter("WikiPageId", WikiPageIdHelper.GetArtifactId(projectId, wikiId, pageId))
          };
          requestContext.GetService<INotificationSubscriptionService>().CreateSubscription(requestContext, createParameters);
          ciEvent.Properties.AddOrIncrement("AutoFollowPage", 1L);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15250811, "Wiki", traceLayer, ex);
        ciEvent.Properties.AddOrIncrement("AutoFollowPageFailed", 1L);
      }
    }

    public static List<NotificationSubscription> GetFollowSubscriptions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      int pageId,
      Guid? subscriberId,
      TimedCiEvent ciEvent)
    {
      using (new StopWatchHelper(ciEvent, nameof (GetFollowSubscriptions)))
      {
        string artifactId = WikiPageIdHelper.GetArtifactId(projectId, wikiId, pageId);
        INotificationSubscriptionService service = requestContext.GetService<INotificationSubscriptionService>();
        SubscriptionQuery subscriptionQuery1 = new SubscriptionQuery()
        {
          Conditions = (IEnumerable<SubscriptionQueryCondition>) new List<SubscriptionQueryCondition>()
          {
            new SubscriptionQueryCondition()
            {
              SubscriberId = subscriberId,
              Filter = (ISubscriptionFilter) new ArtifactFilter("WikiPageId", artifactId)
            }
          },
          QueryFlags = new SubscriptionQueryFlags?(SubscriptionQueryFlags.AlwaysReturnBasicInformation)
        };
        IVssRequestContext tfsRequestContext = requestContext;
        SubscriptionQuery subscriptionQuery2 = subscriptionQuery1;
        return service.QuerySubscriptions(tfsRequestContext, subscriptionQuery2);
      }
    }

    public static int GetAggregationJobDelayTime(DateTime baseTime, int delayInMinutes) => delayInMinutes - baseTime.Minute % delayInMinutes;
  }
}
