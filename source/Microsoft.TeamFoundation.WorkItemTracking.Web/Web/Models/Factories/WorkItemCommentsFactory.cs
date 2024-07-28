// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.WorkItemCommentsFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Mention.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories
{
  internal static class WorkItemCommentsFactory
  {
    internal static CommentList Create(
      WorkItemTrackingRequestContext witRequestContext,
      Guid projectId,
      int workItemId,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComments commentsList,
      bool returnProjectScopedUrl = true,
      int? top = null,
      bool? includeDeleted = null,
      CommentExpandOptions expandOptions = CommentExpandOptions.None,
      CommentSortOrder? order = CommentSortOrder.Desc,
      bool includeSelfUrls = true)
    {
      IEnumerable<Guid> vsids = commentsList.Comments.SelectMany<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment, IEnumerable<Guid>>) (c => (IEnumerable<Guid>) WorkItemCommentsFactory.GetVsids(c.CreatedBy, c.ModifiedBy, c.CreatedOnBehalfOf)));
      IDictionary<Guid, IdentityRef> identityReferencesById = IdentityRefBuilder.Create(witRequestContext.RequestContext, vsids, true, true);
      IEnumerable<CommentMention> allMentions = (IEnumerable<CommentMention>) null;
      HashSet<int> hashSet = commentsList.Comments.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment, int>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment, int>) (c => c.CommentId)).ToHashSet<int>();
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionsByCommentIds = witRequestContext.RequestContext.GetService<ITeamFoundationMentionService>().GetMentionsByCommentIds(witRequestContext.RequestContext, (ISet<int>) hashSet);
      if (WorkItemTrackingFeatureFlags.IsCommentFactoryMentionPerfFixEnabled(witRequestContext.RequestContext) || mentionsByCommentIds != null && mentionsByCommentIds.Any<Microsoft.TeamFoundation.Mention.Server.Mention>())
        allMentions = mentionsByCommentIds.Select<Microsoft.TeamFoundation.Mention.Server.Mention, CommentMention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, CommentMention>) (m => WorkItemCommentFactory.CreateMention((ISecuredObject) commentsList, m)));
      CommentList commentList = new CommentList((ISecuredObject) commentsList)
      {
        Comments = commentsList.Comments.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment, Comment>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment, Comment>) (comment =>
        {
          IVssRequestContext requestContext = witRequestContext.RequestContext;
          Guid projectId1 = projectId;
          IDictionary<Guid, IdentityRef> identityRefs = identityReferencesById;
          Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment comment1 = comment;
          int num = includeSelfUrls ? 1 : 0;
          IEnumerable<CommentMention> source = allMentions;
          IEnumerable<CommentMention> mentions = source != null ? source.Where<CommentMention>((Func<CommentMention, bool>) (m =>
          {
            int? commentId3 = m.CommentId;
            int commentId4 = comment.CommentId;
            return commentId3.GetValueOrDefault() == commentId4 & commentId3.HasValue;
          })) : (IEnumerable<CommentMention>) null;
          return WorkItemCommentFactory.Create(requestContext, projectId1, identityRefs, comment1, num != 0, mentions);
        })),
        Count = commentsList.Comments.Count<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>(),
        TotalCount = commentsList.TotalCount
      };
      if (!string.IsNullOrEmpty(commentsList.ContinuationToken))
      {
        commentList.ContinuationToken = commentsList.ContinuationToken;
        commentList.NextPage = WitUrlHelper.GetWorkItemCommentNextPageUrl(witRequestContext, projectId, workItemId, commentsList.ContinuationToken, top, includeDeleted, expandOptions, order);
      }
      return commentList;
    }

    internal static IList<CommentVersion> Create(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<WorkItemCommentVersion> commentVersions,
      bool includeSelfUrls = true)
    {
      IEnumerable<Guid> vsids = commentVersions.SelectMany<WorkItemCommentVersion, Guid>((Func<WorkItemCommentVersion, IEnumerable<Guid>>) (c => (IEnumerable<Guid>) WorkItemCommentsFactory.GetVsids(c.CreatedBy, c.ModifiedBy, c.CreatedOnBehalfOf)));
      IDictionary<Guid, IdentityRef> identityReferencesById = IdentityRefBuilder.Create(requestContext, vsids, true, true);
      return (IList<CommentVersion>) commentVersions.Select<WorkItemCommentVersion, CommentVersion>((Func<WorkItemCommentVersion, CommentVersion>) (version => WorkItemCommentFactory.Create(requestContext, projectId, identityReferencesById, version, includeSelfUrls))).ToList<CommentVersion>();
    }

    private static List<Guid> GetVsids(Guid createdBy, Guid modifiedBy, string createdOnBehalfOf)
    {
      List<Guid> vsids = new List<Guid>()
      {
        createdBy,
        modifiedBy
      };
      Guid result;
      if (Guid.TryParse(createdOnBehalfOf, out result) && createdBy != result)
        vsids.Add(result);
      return vsids;
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComments Create(
      WorkItemTrackingRequestContext witRequestContext,
      Guid? projectId,
      int id,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments comments,
      bool returnProjectScopedUrl = true)
    {
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComment> source = WorkItemCommentFactory.Create(witRequestContext, projectId, id, comments.Comments, returnProjectScopedUrl);
      return new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComments((ISecuredObject) comments)
      {
        Count = source.Count<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComment>(),
        TotalCount = comments.TotalCount,
        FromRevisionCount = comments.FromRevisionCount,
        Comments = source
      };
    }
  }
}
