// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemCommentReactionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments
{
  public class WorkItemCommentReactionService : IWorkItemCommentReactionService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public WorkItemCommentReaction CreateCommentReaction(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId,
      SocialEngagementType socialEngagementType)
    {
      requestContext.TraceSerializedConditionally(100160601, TraceLevel.Info, "ReactionService", "Service", "Create reaction received the following input data {0}: {1}, {2}: {3}, {4}: {5}", (object) nameof (workItemId), (object) workItemId, (object) nameof (commentId), (object) commentId, (object) nameof (socialEngagementType), (object) socialEngagementType);
      CommentReaction commentReaction = requestContext.GetService<ICommentReactionService>().CreateCommentReaction(requestContext, projectId, WorkItemArtifactKinds.WorkItem, workItemId.ToString(), commentId, socialEngagementType);
      requestContext.GetService<ITeamFoundationWorkItemService>().CreateOrUpdateWorkItemReactionsAggregateCount(requestContext, workItemId, socialEngagementType, 1);
      this.PublishActionCi(requestContext, workItemId, commentId, socialEngagementType, nameof (CreateCommentReaction));
      return new WorkItemCommentReaction(commentReaction, commentReaction.SecuredObject.RequiredPermissions, commentReaction.SecuredObject.GetToken());
    }

    public WorkItemCommentReaction DeleteCommentReaction(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId,
      SocialEngagementType socialEngagementType)
    {
      requestContext.TraceSerializedConditionally(100160602, TraceLevel.Info, "ReactionService", "Service", "Delete reaction received the following input data {0}: {1}, {2}: {3}, {4}: {5}", (object) nameof (workItemId), (object) workItemId, (object) nameof (commentId), (object) commentId, (object) nameof (socialEngagementType), (object) socialEngagementType);
      CommentReaction reaction = requestContext.GetService<ICommentReactionService>().DeleteCommentReaction(requestContext, projectId, WorkItemArtifactKinds.WorkItem, workItemId.ToString(), commentId, socialEngagementType);
      requestContext.GetService<ITeamFoundationWorkItemService>().CreateOrUpdateWorkItemReactionsAggregateCount(requestContext, workItemId, socialEngagementType, -1);
      this.PublishActionCi(requestContext, workItemId, commentId, socialEngagementType, nameof (DeleteCommentReaction));
      return new WorkItemCommentReaction(reaction, reaction.SecuredObject.RequiredPermissions, reaction.SecuredObject.GetToken());
    }

    public IList<Tuple<int, int>> GetSortedWorkItemReactionsCount(
      IVssRequestContext requestContext,
      HashSet<int> workItemIds,
      SocialEngagementType socialEngagementType)
    {
      List<Tuple<int, int>> itemReactionsCount1 = new List<Tuple<int, int>>();
      requestContext.TraceSerializedConditionally(100160604, TraceLevel.Info, "ReactionService", "Service", "{0} received the following input parameters- {1}: {2}, {3}: {4}", (object) nameof (GetSortedWorkItemReactionsCount), (object) nameof (workItemIds), (object) workItemIds, (object) nameof (socialEngagementType), (object) socialEngagementType);
      if (workItemIds.Count == 0)
      {
        requestContext.Trace(100160301, TraceLevel.Warning, "ReactionService", "Service", "There are no valid work item ids to get the reactions count for");
      }
      else
      {
        foreach (WorkItemReactionsCount itemReactionsCount2 in requestContext.GetService<ITeamFoundationWorkItemService>().GetSortedWorkItemReactionsCount(requestContext, (IEnumerable<int>) workItemIds, socialEngagementType))
          itemReactionsCount1.Add(Tuple.Create<int, int>(itemReactionsCount2.WorkItemId, itemReactionsCount2.TotalReactionsCount));
      }
      return (IList<Tuple<int, int>>) itemReactionsCount1;
    }

    public IEnumerable<IdentityRef> GetEngagedUsers(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId,
      SocialEngagementType reactionType,
      int top = 20,
      int skip = 0)
    {
      requestContext.TraceSerializedConditionally(100160605, TraceLevel.Info, "ReactionService", "Service", "Delete reaction received the following input data {0}: {1}, {2}: {3}, {4}: {5}", (object) nameof (workItemId), (object) workItemId, (object) nameof (commentId), (object) commentId, (object) nameof (reactionType), (object) reactionType);
      return requestContext.GetService<ICommentReactionService>().GetEngagedUsers(requestContext, projectId, WorkItemArtifactKinds.WorkItem, workItemId.ToString(), commentId, reactionType, top, skip);
    }

    private void PublishActionCi(
      IVssRequestContext requestContext,
      int workItemId,
      int commentId,
      SocialEngagementType socialEngagementType,
      string action)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add(nameof (workItemId), (double) workItemId);
      ciData.Add(nameof (commentId), (double) commentId);
      ciData.Add(nameof (socialEngagementType), (object) socialEngagementType);
      WorkItemCommentReactionService.PublishCiData(requestContext, ciData, action);
    }

    private static void PublishCiData(
      IVssRequestContext requestContext,
      CustomerIntelligenceData ciData,
      string action)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      ciData.Add(nameof (action), action);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = ciData;
      service.Publish(requestContext1, "ReactionService", "CommentReactions", properties);
    }
  }
}
