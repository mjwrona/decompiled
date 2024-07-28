// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.WorkItemCommentFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Mention.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories
{
  internal static class WorkItemCommentFactory
  {
    private static readonly Version MaximumVersionToReturnIdentityFromCommentRecord = new Version(4, 1);

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment Create(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment comment,
      bool includeUrls = true,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentMention> mentions = null)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>(comment, nameof (comment));
      List<Guid> vsids = WorkItemCommentFactory.GetVsids(comment.CreatedBy, comment.ModifiedBy, comment.CreatedOnBehalfOf);
      IDictionary<Guid, IdentityRef> identityRefs = IdentityRefBuilder.Create(requestContext, (IEnumerable<Guid>) vsids, includeUrls, true);
      return WorkItemCommentFactory.Create(requestContext, projectId, identityRefs, comment, includeUrls, mentions);
    }

    internal static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction> CreateCommentReactions(
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment comment)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>(comment, nameof (comment));
      ArgumentUtility.CheckForNull<IReadOnlyCollection<WorkItemCommentReaction>>(comment.Reactions, "Reactions");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return comment.Reactions.Select<WorkItemCommentReaction, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction>(WorkItemCommentFactory.\u003C\u003EO.\u003C0\u003E__CreateReactions ?? (WorkItemCommentFactory.\u003C\u003EO.\u003C0\u003E__CreateReactions = new Func<WorkItemCommentReaction, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction>(WorkItemCommentFactory.CreateReactions)));
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction CreateCommentReaction(
      Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction commentReaction)
    {
      ArgumentUtility.CheckForNull<Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction>(commentReaction, nameof (commentReaction));
      return new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction((ISecuredObject) commentReaction)
      {
        Type = (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReactionType) commentReaction.Type,
        Count = commentReaction.Count,
        IsCurrentUserEngaged = commentReaction.IsCurrentUserEngaged,
        CommentId = commentReaction.CommentId
      };
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction CreateReactions(
      WorkItemCommentReaction reaction)
    {
      ArgumentUtility.CheckForNull<WorkItemCommentReaction>(reaction, nameof (reaction));
      return new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction((ISecuredObject) reaction)
      {
        Type = (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReactionType) reaction.Type,
        Count = reaction.Count,
        IsCurrentUserEngaged = reaction.IsCurrentUserEngaged,
        CommentId = reaction.CommentId
      };
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentMention CreateMention(
      ISecuredObject securedObject,
      Microsoft.TeamFoundation.Mention.Server.Mention mention)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentMention(securedObject)
      {
        ArtifactId = mention.ArtifactId,
        ArtifactType = mention.ArtifactType,
        CommentId = mention.CommentId,
        TargetId = mention.TargetId
      };
    }

    private static void AugmentWorkItemCommentResponse(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment result,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment comment)
    {
      if (comment.Reactions.Any<WorkItemCommentReaction>())
        result.Reactions = WorkItemCommentFactory.CreateCommentReactions(comment);
      if (result.Mentions != null)
        return;
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionsByCommentIds = requestContext.GetService<ITeamFoundationMentionService>().GetMentionsByCommentIds(requestContext, (ISet<int>) new HashSet<int>()
      {
        comment.CommentId
      });
      if (!mentionsByCommentIds.Any<Microsoft.TeamFoundation.Mention.Server.Mention>())
        return;
      result.Mentions = mentionsByCommentIds.Select<Microsoft.TeamFoundation.Mention.Server.Mention, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentMention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentMention>) (m => WorkItemCommentFactory.CreateMention((ISecuredObject) comment, m)));
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment Create(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<Guid, IdentityRef> identityRefs,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment comment,
      bool includeUrls,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentMention> mentions = null)
    {
      ArgumentUtility.CheckForNull<IDictionary<Guid, IdentityRef>>(identityRefs, nameof (identityRefs));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>(comment, nameof (comment));
      IdentityRef fallbackToConstant1 = WorkItemCommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, (ISecuredObject) comment, comment.ModifiedBy, string.Empty, identityRefs);
      IdentityRef fallbackToConstant2 = WorkItemCommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, (ISecuredObject) comment, comment.CreatedBy, string.Empty, identityRefs);
      IdentityRef createdOnBehalfOf = WorkItemCommentFactory.GetCreatedOnBehalfOf(requestContext, (ISecuredObject) comment, identityRefs, comment.CreatedBy, comment.CreatedOnBehalfOf);
      DateTime? createdOnBehalfDate = WorkItemCommentFactory.GetCreatedOnBehalfDate(comment.CreatedDate, comment.CreatedOnBehalfDate);
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment comment1 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment((ISecuredObject) comment);
      comment1.WorkItemId = comment.WorkItemId;
      comment1.Id = comment.CommentId;
      comment1.Version = comment.Version;
      comment1.CreatedDate = comment.CreatedDate;
      comment1.CreatedBy = fallbackToConstant2;
      comment1.CreatedOnBehalfOf = createdOnBehalfOf;
      comment1.CreatedOnBehalfDate = createdOnBehalfDate;
      comment1.Format = (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentFormat) comment.Format;
      comment1.Mentions = mentions;
      comment1.ModifiedDate = comment.ModifiedDate;
      comment1.ModifiedBy = fallbackToConstant1;
      comment1.Text = comment.Text;
      comment1.IsDeleted = comment.IsDeleted;
      comment1.Url = includeUrls ? WitUrlHelper.GetWorkItemCommentResponseUrl(requestContext, projectId, comment.WorkItemId, comment.CommentId) : (string) null;
      comment1.RenderedText = comment.RenderedText;
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment result = comment1;
      WorkItemCommentFactory.AugmentWorkItemCommentResponse(requestContext, result, comment);
      return result;
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentVersion Create(
      IVssRequestContext requestContext,
      Guid projectId,
      WorkItemCommentVersion commentVersion,
      bool includeUrls = true)
    {
      ArgumentUtility.CheckForNull<WorkItemCommentVersion>(commentVersion, nameof (commentVersion));
      List<Guid> vsids = WorkItemCommentFactory.GetVsids(commentVersion.CreatedBy, commentVersion.ModifiedBy, commentVersion.CreatedOnBehalfOf);
      IDictionary<Guid, IdentityRef> identityRefs = IdentityRefBuilder.Create(requestContext, (IEnumerable<Guid>) vsids, includeUrls, true);
      return WorkItemCommentFactory.Create(requestContext, projectId, identityRefs, commentVersion, includeUrls);
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentVersion Create(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<Guid, IdentityRef> identityRefs,
      WorkItemCommentVersion commentVersion,
      bool includeUrls)
    {
      ArgumentUtility.CheckForNull<IDictionary<Guid, IdentityRef>>(identityRefs, nameof (identityRefs));
      ArgumentUtility.CheckForNull<WorkItemCommentVersion>(commentVersion, nameof (commentVersion));
      IdentityRef fallbackToConstant1 = WorkItemCommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, (ISecuredObject) commentVersion, commentVersion.ModifiedBy, string.Empty, identityRefs);
      IdentityRef fallbackToConstant2 = WorkItemCommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, (ISecuredObject) commentVersion, commentVersion.CreatedBy, string.Empty, identityRefs);
      IdentityRef createdOnBehalfOf = WorkItemCommentFactory.GetCreatedOnBehalfOf(requestContext, (ISecuredObject) commentVersion, identityRefs, commentVersion.CreatedBy, commentVersion.CreatedOnBehalfOf);
      DateTime? createdOnBehalfDate = WorkItemCommentFactory.GetCreatedOnBehalfDate(commentVersion.CreatedDate, new DateTime?(commentVersion.CreatedOnBehalfDate));
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentVersion commentVersion1 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentVersion((ISecuredObject) commentVersion);
      commentVersion1.Id = commentVersion.CommentId;
      commentVersion1.Version = commentVersion.Version;
      commentVersion1.Text = commentVersion.Text;
      commentVersion1.RenderedText = commentVersion.RenderedText;
      commentVersion1.CreatedDate = commentVersion.CreatedDate;
      commentVersion1.CreatedBy = fallbackToConstant2;
      commentVersion1.CreatedOnBehalfOf = createdOnBehalfOf;
      commentVersion1.CreatedOnBehalfDate = createdOnBehalfDate;
      commentVersion1.ModifiedDate = commentVersion.ModifiedDate;
      commentVersion1.ModifiedBy = fallbackToConstant1;
      commentVersion1.IsDeleted = commentVersion.IsDeleted;
      commentVersion1.Url = includeUrls ? WitUrlHelper.GetWorkItemCommentVersionUrl(requestContext, projectId, commentVersion.WorkItemId, commentVersion.CommentId, commentVersion.Version) : (string) null;
      return commentVersion1;
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment Create(
      IVssRequestContext requestContext,
      Guid projectId,
      WorkItemCommentVersionRecord comment,
      ISecuredObject securedObject,
      bool includeUrls = true)
    {
      ArgumentUtility.CheckForNull<WorkItemCommentVersionRecord>(comment, nameof (comment));
      List<Guid> vsids = WorkItemCommentFactory.GetVsids(comment.CreatedBy, comment.ModifiedBy, comment.CreatedOnBehalfOf);
      IDictionary<Guid, IdentityRef> identityRefs = IdentityRefBuilder.Create(requestContext, (IEnumerable<Guid>) vsids, true, true);
      return WorkItemCommentFactory.Create(requestContext, projectId, identityRefs, comment, securedObject, includeUrls);
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment Create(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<Guid, IdentityRef> identityRefs,
      WorkItemCommentVersionRecord comment,
      ISecuredObject securedObject,
      bool includeUrls)
    {
      ArgumentUtility.CheckForNull<WorkItemCommentVersionRecord>(comment, nameof (comment));
      ArgumentUtility.CheckForNull<IDictionary<Guid, IdentityRef>>(identityRefs, nameof (identityRefs));
      IdentityRef fallbackToConstant1 = WorkItemCommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, securedObject, comment.ModifiedBy, string.Empty, identityRefs);
      IdentityRef fallbackToConstant2 = WorkItemCommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, securedObject, comment.CreatedBy, string.Empty, identityRefs);
      IdentityRef createdOnBehalfOf = WorkItemCommentFactory.GetCreatedOnBehalfOf(requestContext, securedObject, identityRefs, comment.CreatedBy, comment.CreatedOnBehalfOf);
      DateTime? createdOnBehalfDate = WorkItemCommentFactory.GetCreatedOnBehalfDate(comment.CreatedDate, new DateTime?(comment.CreatedOnBehalfDate));
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment comment1 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment(securedObject);
      comment1.WorkItemId = comment.WorkItemId;
      comment1.Id = comment.CommentId;
      comment1.Version = comment.Version;
      comment1.CreatedDate = comment.CreatedDate;
      comment1.CreatedBy = fallbackToConstant2;
      comment1.CreatedOnBehalfOf = createdOnBehalfOf;
      comment1.CreatedOnBehalfDate = createdOnBehalfDate;
      comment1.ModifiedDate = comment.ModifiedDate;
      comment1.ModifiedBy = fallbackToConstant1;
      comment1.Text = comment.Text;
      comment1.IsDeleted = comment.IsDeleted;
      comment1.Url = includeUrls ? WitUrlHelper.GetWorkItemCommentResponseUrl(requestContext, projectId, comment.WorkItemId, comment.CommentId) : (string) null;
      return comment1;
    }

    private static IdentityRef GetCreatedOnBehalfOf(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      IDictionary<Guid, IdentityRef> identityRefs,
      Guid createdBy,
      string createdOnBehalfOfValue)
    {
      IdentityRef createdOnBehalfOf = (IdentityRef) null;
      Guid result;
      Guid.TryParse(createdOnBehalfOfValue, out result);
      if (result != createdBy)
        createdOnBehalfOf = WorkItemCommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, securedObject, result, createdOnBehalfOfValue, identityRefs);
      return createdOnBehalfOf;
    }

    private static DateTime? GetCreatedOnBehalfDate(
      DateTime createdDate,
      DateTime? createdOnBehalfDate)
    {
      DateTime? createdOnBehalfDate1 = new DateTime?();
      DateTime? nullable = createdOnBehalfDate;
      DateTime dateTime = createdDate;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != dateTime ? 1 : 0) : 0) : 1) != 0)
        createdOnBehalfDate1 = createdOnBehalfDate;
      return createdOnBehalfDate1;
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

    internal static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComment> Create(
      WorkItemTrackingRequestContext witRequestContext,
      Guid? projectId,
      int workItemId,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment> comments,
      bool returnProjectScopedUrl = true)
    {
      IDictionary<Guid, IdentityRef> identityRefsById = IdentityRefBuilder.Create(witRequestContext.RequestContext, (IEnumerable<Guid>) comments.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment, Guid>) (c => c.CreatedByTeamFoundationId)).ToList<Guid>(), true, true);
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment comment in comments)
      {
        if (!string.IsNullOrEmpty(comment.Text))
          yield return WorkItemCommentFactory.Create(witRequestContext, projectId, workItemId, comment, identityRefsById, returnProjectScopedUrl);
      }
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComment Create(
      WorkItemTrackingRequestContext witRequestContext,
      Guid? projectId,
      int workItemId,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment comment,
      IDictionary<Guid, IdentityRef> identityRefsById,
      bool returnProjectScopedUrl = true)
    {
      string displayName = !string.IsNullOrWhiteSpace(comment.CreatedByDisplayName) ? comment.CreatedByDisplayName : comment.CreatedByDisplayPart;
      IdentityReference identityReference;
      if (WorkItemCommentFactory.ShouldReturnIdentityFromCommentRecord(witRequestContext.RequestContext))
        identityReference = new IdentityReference(new IdentityRef()
        {
          Id = comment.CreatedByTeamFoundationId.ToString(),
          DisplayName = displayName,
          Url = IdentityHelper.GetIdentityResourceUriString(witRequestContext.RequestContext, comment.CreatedByTeamFoundationId)
        }, comment.CreatedByDisplayPart);
      else
        identityReference = new IdentityReference(WorkItemCommentFactory.GetIdentityRefWithFallbackToConstant(witRequestContext.RequestContext, (ISecuredObject) comment, comment.CreatedByTeamFoundationId, displayName, identityRefsById));
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComment workItemComment = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComment((ISecuredObject) comment);
      workItemComment.Revision = comment.RevisionId;
      workItemComment.Url = WitUrlHelper.GetWorkItemCommentUrl(witRequestContext, projectId, workItemId, new int?(comment.RevisionId), returnProjectScopedUrl);
      workItemComment.RevisedBy = identityReference;
      workItemComment.RevisedDate = comment.RevisionDate;
      workItemComment.Text = comment.Text;
      workItemComment.RenderedText = comment.RenderedText;
      workItemComment.Format = (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentFormat) comment.Format;
      return workItemComment;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.CommentSortOrder ToServerCommentSortOrder(
      this Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentSortOrder order)
    {
      return order == Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentSortOrder.Asc ? Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.CommentSortOrder.Asc : Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.CommentSortOrder.Desc;
    }

    private static bool ShouldReturnIdentityFromCommentRecord(IVssRequestContext requestContext)
    {
      ApiResourceVersion apiResourceVersion;
      return requestContext.TryGetItem<ApiResourceVersion>("WitApiResourceVersion", out apiResourceVersion) && apiResourceVersion.ApiVersion <= WorkItemCommentFactory.MaximumVersionToReturnIdentityFromCommentRecord;
    }

    private static IdentityRef GetIdentityRefWithFallbackToConstant(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Guid identityId,
      string displayName,
      IDictionary<Guid, IdentityRef> identityRefsById)
    {
      IdentityRef fallbackToConstant;
      if (!identityRefsById.TryGetValue(identityId, out fallbackToConstant))
      {
        if (CommonWITUtils.CanAccessCrossProjectWorkItems(requestContext))
        {
          ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef(securedObject);
          constantIdentityRef.Id = identityId.ToString();
          constantIdentityRef.DisplayName = displayName;
          fallbackToConstant = (IdentityRef) constantIdentityRef;
        }
        else
        {
          string str = InternalsResourceStrings.Get("UnknownUser");
          ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef(securedObject);
          constantIdentityRef.Id = identityId.ToString();
          constantIdentityRef.DisplayName = str;
          fallbackToConstant = (IdentityRef) constantIdentityRef;
        }
      }
      return fallbackToConstant;
    }
  }
}
