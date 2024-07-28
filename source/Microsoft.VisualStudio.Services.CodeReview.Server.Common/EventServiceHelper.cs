// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.EventServiceHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.Events;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public class EventServiceHelper
  {
    public static void SendRealtimeEvent(
      IVssRequestContext requestContext,
      CodeReviewEventNotification realtimeEvent,
      string area,
      string layer)
    {
      requestContext.Trace(1380900, TraceLevel.Verbose, area, layer, "Code review: id: '{0}', project id: '{1}', Source ArtifactId: '{2}'", (object) realtimeEvent.ReviewId, (object) realtimeEvent.ProjectId, (object) realtimeEvent.SourceArtifactId);
      try
      {
        requestContext.GetService<ICodeReviewDispatcher>().SendRealtimeEvent(requestContext, realtimeEvent);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1380901, area, layer, ex);
      }
    }

    public static void PublishNotification(
      IVssRequestContext requestContext,
      DiscussionsNotification notificationEvent,
      string area,
      string layer)
    {
      foreach (DiscussionThread thread in notificationEvent.Threads)
      {
        DiscussionThread discussionThread = ((IEnumerable<DiscussionThread>) notificationEvent.Threads).First<DiscussionThread>();
        Guid projectId;
        int reviewId;
        DiscussionExtensions.ExtractMetadata(discussionThread.ArtifactUri, out projectId, out reviewId);
        if (reviewId > 0)
        {
          CommentThread reviewCommentThread = thread.ToReviewCommentThread(projectId, reviewId);
          CommentNotification realtimeEvent = new CommentNotification(projectId, reviewId, reviewCommentThread, new DateTime?(thread.PriorLastUpdatedDate), new DateTime?(discussionThread.LastUpdatedDate));
          EventServiceHelper.SendRealtimeEvent(requestContext, (CodeReviewEventNotification) realtimeEvent, area, layer);
        }
      }
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
    }
  }
}
