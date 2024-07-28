// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WorkItemHub.WorkItemChangeSubscriber
// Assembly: Microsoft.Azure.Boards.WorkItemHub, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 749A696A-54F8-4B6F-8877-B350F1725C24
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.WorkItemHub.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Comments.Server.Events;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Boards.WorkItemHub
{
  public class WorkItemChangeSubscriber : ISubscriber
  {
    private const string ReactionNotificationThrottleSize = "/Service/Reactions/Settings/NotificationThrottleSize";
    private const int DefaultReactionNotificationThrottleSize = 5;
    private static readonly Type[] SubscribedTypes = new Type[2]
    {
      typeof (WorkItemChangedEvent),
      typeof (CommentReactionChangedEvent)
    };

    public string Name => nameof (WorkItemChangeSubscriber);

    public SubscriberPriority Priority => SubscriberPriority.Normal;

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      statusMessage = (string) null;
      properties = (ExceptionPropertyCollection) null;
      if (notificationType != NotificationType.DecisionPoint)
      {
        using (new TraceWatch(requestContext, 290963, TraceLevel.Info, TimeSpan.FromMilliseconds(10.0), "Agile", TfsTraceLayers.BusinessLogic, "ProcessWorkItemChangeEvent", Array.Empty<object>()))
        {
          try
          {
            WorkItemChangedEvent workItemChangedEvent = notificationEventArgs as WorkItemChangedEvent;
            IWorkItemHubDispatcher service = requestContext.GetService<IWorkItemHubDispatcher>();
            if (workItemChangedEvent != null)
              service.NotifyWorkItemsChanged(requestContext, workItemChangedEvent);
            else if (notificationEventArgs is CommentReactionChangedEvent commentReactionChangedEvent)
            {
              if (commentReactionChangedEvent != null)
              {
                if (commentReactionChangedEvent.ArtifactKind == WorkItemArtifactKinds.WorkItem)
                {
                  if (requestContext.IsFeatureEnabled("Comments.Server.EnableCommentReactionsNotifications") && commentReactionChangedEvent.IsReactionAdded)
                    this.FireCommentReactionAddedEvent(requestContext, commentReactionChangedEvent);
                  if (requestContext.IsFeatureEnabled("Comments.Server.EnableLiveUpdatesForCommentReactions"))
                    service.NotifyWorkItemsChanged(requestContext, commentReactionChangedEvent);
                }
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(290964, "Agile", TfsTraceLayers.BusinessLogic, ex);
          }
        }
      }
      return EventNotificationStatus.ActionPermitted;
    }

    private void FireCommentReactionAddedEvent(
      IVssRequestContext requestContext,
      CommentReactionChangedEvent commentReactionChangedEvent)
    {
      int totalReactionsCount = commentReactionChangedEvent.TotalReactionsCount;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Reactions/Settings/NotificationThrottleSize", true, 5);
      if (totalReactionsCount != 1 && (num == 0 || totalReactionsCount % num != 0))
        return;
      Guid projectId = commentReactionChangedEvent.ProjectId;
      Guid artifactKind = commentReactionChangedEvent.ArtifactKind;
      string artifactId = commentReactionChangedEvent.ArtifactId;
      int commentId = commentReactionChangedEvent.CommentId;
      Comment comment = requestContext.GetService<ICommentService>().GetComment(requestContext, projectId, artifactKind, artifactId, commentId);
      Guid reactionChangedByUser = Guid.Parse(commentReactionChangedEvent.ReactionModifiedBy.Id);
      Guid guid = comment.CreatedBy;
      Guid result;
      if (Guid.TryParse(comment.CreatedOnBehalfOf, out result) && guid != result)
        guid = result;
      List<Guid> list = ((IEnumerable<Guid>) new Guid[2]
      {
        guid,
        comment.ModifiedBy
      }).Where<Guid>((Func<Guid, bool>) (recipient => recipient != reactionChangedByUser)).Distinct<Guid>().ToList<Guid>();
      requestContext.Trace(290965, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, string.Format("Reaction recipients info - {0}: {1},", (object) "reactionChangedByUser", (object) reactionChangedByUser) + string.Format(" {0}: {1}, {2}: {3}", (object) "CreatedBy", (object) comment.CreatedBy, (object) "ModifiedBy", (object) comment.ModifiedBy));
      IDisposableReadOnlyList<ICommentProvider> extensions = requestContext.GetExtensions<ICommentProvider>(ExtensionLifetime.Service);
      ICommentProvider commentProvider = extensions != null ? extensions.FirstOrDefault<ICommentProvider>((Func<ICommentProvider, bool>) (p => p.ArtifactKind == artifactKind)) : (ICommentProvider) null;
      if (commentProvider == null)
        throw new CommentProviderNotRegisteredException(artifactKind);
      if (!this.CanSendEmailToRecipients(requestContext, commentProvider, list, projectId, artifactId, artifactKind))
        return;
      IdentityRef reactionModifiedBy = commentReactionChangedEvent.ReactionModifiedBy;
      VssNotificationEvent theEvent = new VssNotificationEvent()
      {
        EventType = "ms.vss-work.workitem-reaction-added-event"
      };
      foreach (Guid id in list)
        theEvent.AddActor("recipient", id);
      ArtifactInfo artifactInfo = commentProvider.GetArtifactInfo(requestContext, projectId, artifactId);
      string str = commentProvider.ArtifactFriendlyName;
      object obj;
      if (artifactInfo.ArtifactProperties != null && artifactInfo.ArtifactProperties.TryGetValue("Type", out obj))
        str = string.Format("{0} {1}", obj, (object) artifactId);
      WorkItemCommentReactionAddedEvent reactionAddedEvent = new WorkItemCommentReactionAddedEvent()
      {
        ReactionAddedBy = reactionModifiedBy,
        ReactionTypeIcon = commentReactionChangedEvent.ReactionTypeIcon,
        ArtifactKindFriendlyName = commentProvider.ArtifactFriendlyName,
        ArtifactType = str,
        ArtifactInfo = artifactInfo
      };
      theEvent.Data = (object) reactionAddedEvent;
      requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ArtifactType", commentProvider.ArtifactFriendlyName);
      properties.Add("ArtifactId", artifactId);
      properties.Add("CommentId", (double) commentId);
      properties.Add("ReactionType", commentReactionChangedEvent.ReactionType);
      properties.Add("EmailRecipients", (object) list);
      properties.Add("ReactionAddedBy", reactionModifiedBy.Id);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "CommentService", "ReactionsEmailNotification", properties);
    }

    private bool CanSendEmailToRecipients(
      IVssRequestContext requestContext,
      ICommentProvider commentProvider,
      List<Guid> emailRecipients,
      Guid projectId,
      string artifactId,
      Guid artifactKind)
    {
      if (emailRecipients.Count == 0)
        return false;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) emailRecipients, QueryMembership.None, (IEnumerable<string>) null);
      return source != null && emailRecipients.Count == source.Count && source.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => commentProvider.CanReadArtifact(requestContext, identity, projectId, artifactId)));
    }

    Type[] ISubscriber.SubscribedTypes() => WorkItemChangeSubscriber.SubscribedTypes;
  }
}
