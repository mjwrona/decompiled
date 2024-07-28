// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.IWorkItemCommentReactionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments
{
  [DefaultServiceImplementation(typeof (WorkItemCommentReactionService))]
  public interface IWorkItemCommentReactionService : IVssFrameworkService
  {
    WorkItemCommentReaction CreateCommentReaction(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId,
      SocialEngagementType socialEngagementType);

    WorkItemCommentReaction DeleteCommentReaction(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId,
      SocialEngagementType socialEngagementType);

    IList<Tuple<int, int>> GetSortedWorkItemReactionsCount(
      IVssRequestContext requestContext,
      HashSet<int> workItemIds,
      SocialEngagementType socialEngagementType);

    IEnumerable<IdentityRef> GetEngagedUsers(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId,
      SocialEngagementType reactionType,
      int top = 20,
      int skip = 0);
  }
}
