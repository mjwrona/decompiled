// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.Events.CommentEvent
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server.Events
{
  public class CommentEvent
  {
    internal static IEnumerable<CommentEvent> FireCommentEvents(
      IVssRequestContext requestContext,
      Guid projectId,
      ICommentProvider commentProvider,
      IEnumerable<Comment> comments,
      CommentEventType commentEventType,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<ICommentProvider>(commentProvider, nameof (commentProvider));
      List<CommentEvent> eventServiceEvents = (List<CommentEvent>) null;
      if (commentProvider.SupportsMentions && comments != null)
      {
        using (requestContext.TraceBlock(140281, 140282, "CommentService", "Service", nameof (FireCommentEvents)))
        {
          try
          {
            eventServiceEvents = CommentEvent.Create(requestContext, projectId, commentProvider, comments, commentEventType, commentProvider.ArtifactFriendlyName, suppressNotifications).ToList<CommentEvent>();
            requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((taskRequestContext, ignored) =>
            {
              using (taskRequestContext.TraceBlock(140285, 140285, "CommentService", "Service", "CommentAddedEventCallBack"))
              {
                ITeamFoundationEventService service = taskRequestContext.GetService<ITeamFoundationEventService>();
                foreach (CommentEvent notificationEvent in eventServiceEvents)
                  service.PublishNotification(taskRequestContext, (object) notificationEvent);
              }
            }));
          }
          catch (Exception ex)
          {
            TeamFoundationEventLog.Default.LogException(requestContext, "Failed to queue CommentEvent", ex, 140283, EventLogEntryType.Error);
          }
        }
      }
      return (IEnumerable<CommentEvent>) eventServiceEvents;
    }

    private static IEnumerable<CommentEvent> Create(
      IVssRequestContext requestContext,
      Guid projectId,
      ICommentProvider commentProvider,
      IEnumerable<Comment> comments,
      CommentEventType commentEventType,
      string artifactFriendlyName,
      bool suppressNotifications)
    {
      return comments.Select<Comment, CommentEvent>((Func<Comment, CommentEvent>) (c =>
      {
        ArtifactInfo artifactInfo = commentProvider.GetArtifactInfo(requestContext, projectId, c.ArtifactId);
        return new CommentEvent()
        {
          ArtifactId = c.ArtifactId,
          Comment = c,
          ArtifactKind = c.ArtifactKind,
          ProjectGuid = projectId,
          ArtifactFriendlyName = artifactFriendlyName,
          ArtifactUri = artifactInfo.ArtifactUri,
          CommentEventType = commentEventType,
          ArtifactTitle = artifactInfo.ArtifactTitle,
          SuppressNotifications = suppressNotifications
        };
      }));
    }

    public CommentEventType CommentEventType { get; private set; }

    public Guid ProjectGuid { get; private set; }

    public Guid ArtifactKind { get; private set; }

    public string ArtifactFriendlyName { get; private set; }

    public string ArtifactId { get; private set; }

    public string ArtifactUri { get; private set; }

    public Comment Comment { get; private set; }

    public string ArtifactTitle { get; private set; }

    public bool SuppressNotifications { get; private set; }
  }
}
