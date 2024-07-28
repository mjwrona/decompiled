// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.Web.Factories.CommentFactory
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.Azure.DevOps.Comments.Web.Common;
using Microsoft.Azure.DevOps.Comments.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.Comments.Web.Factories
{
  internal static class CommentFactory
  {
    internal static T Create<TList, T>(IVssRequestContext requestContext, Microsoft.TeamFoundation.Comments.Server.Comment comment)
      where TList : CommentList, new()
      where T : Microsoft.Azure.DevOps.Comments.WebApi.Comment, new()
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Comments.Server.Comment>(comment, nameof (comment));
      List<Guid> vsids = new List<Guid>()
      {
        comment.CreatedBy,
        comment.ModifiedBy
      };
      IDictionary<Guid, IdentityRef> identityRefs = IdentityRefBuilder.Create(requestContext, (IEnumerable<Guid>) vsids, true, true);
      return CommentFactory.Create<TList, T>(requestContext, identityRefs, comment);
    }

    internal static T Create<TList, T>(
      IVssRequestContext requestContext,
      IDictionary<Guid, IdentityRef> identityRefs,
      Microsoft.TeamFoundation.Comments.Server.Comment comment)
      where TList : CommentList, new()
      where T : Microsoft.Azure.DevOps.Comments.WebApi.Comment, new()
    {
      ArgumentUtility.CheckForNull<IDictionary<Guid, IdentityRef>>(identityRefs, nameof (identityRefs));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Comments.Server.Comment>(comment, nameof (comment));
      IdentityRef fallbackToConstant1 = CommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, comment.SecuredObject, comment.ModifiedBy, string.Empty, identityRefs);
      IdentityRef fallbackToConstant2 = CommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, comment.SecuredObject, comment.CreatedBy, string.Empty, identityRefs);
      T obj = new T();
      obj.ArtifactId = comment.ArtifactId;
      obj.Id = comment.CommentId;
      obj.Version = comment.Version;
      obj.CreatedDate = comment.CreatedDate;
      obj.CreatedBy = fallbackToConstant2;
      obj.ModifiedDate = comment.ModifiedDate;
      obj.ModifiedBy = fallbackToConstant1;
      obj.Text = comment.Text;
      obj.RenderedText = comment.RenderedText;
      obj.IsDeleted = comment.IsDeleted;
      obj.State = (Microsoft.Azure.DevOps.Comments.WebApi.CommentState) comment.State;
      obj.Reactions = CommentFactory.CreateCommentReactions(comment);
      obj.Replies = (CommentList) CommentListFactory.Create<TList, T>(requestContext, comment.Children);
      obj.ParentId = comment.ParentId;
      obj.SetSecuredObject(comment.SecuredObject);
      return obj;
    }

    internal static T Create<T>(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      IDictionary<Guid, IdentityRef> identityRefs,
      Microsoft.TeamFoundation.Comments.Server.CommentVersion commentVersion)
      where T : Microsoft.Azure.DevOps.Comments.WebApi.CommentVersion, new()
    {
      ArgumentUtility.CheckForNull<IDictionary<Guid, IdentityRef>>(identityRefs, nameof (identityRefs));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Comments.Server.CommentVersion>(commentVersion, nameof (commentVersion));
      IdentityRef fallbackToConstant1 = CommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, securedObject, commentVersion.ModifiedBy, string.Empty, identityRefs);
      IdentityRef fallbackToConstant2 = CommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, securedObject, commentVersion.CreatedBy, string.Empty, identityRefs);
      T obj = new T();
      obj.Id = commentVersion.CommentId;
      obj.Version = commentVersion.Version;
      obj.Text = commentVersion.Text;
      obj.RenderedText = commentVersion.RenderedText;
      obj.CreatedDate = commentVersion.CreatedDate;
      obj.CreatedBy = fallbackToConstant2;
      obj.ModifiedDate = commentVersion.ModifiedDate;
      obj.ModifiedBy = fallbackToConstant1;
      obj.IsDeleted = commentVersion.IsDeleted;
      obj.State = (Microsoft.Azure.DevOps.Comments.WebApi.CommentState) commentVersion.State;
      obj.SetSecuredObject(commentVersion.SecuredObject);
      return obj;
    }

    internal static T Create<T>(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Comments.Server.CommentAttachment commentAttachment)
      where T : Microsoft.Azure.DevOps.Comments.WebApi.CommentAttachment, new()
    {
      IDictionary<Guid, IdentityRef> identityRefsById = IdentityRefBuilder.Create(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        commentAttachment.CreatedBy
      }, true, true);
      IdentityRef fallbackToConstant = CommentFactory.GetIdentityRefWithFallbackToConstant(requestContext, commentAttachment.SecuredObject, commentAttachment.CreatedBy, string.Empty, identityRefsById);
      T obj = new T();
      obj.Id = commentAttachment.Id;
      obj.CreatedBy = fallbackToConstant;
      obj.CreatedDate = commentAttachment.CreatedDate;
      return obj;
    }

    private static T Create<T>(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Microsoft.TeamFoundation.Comments.Server.CommentVersion commentVersion)
      where T : Microsoft.Azure.DevOps.Comments.WebApi.CommentVersion, new()
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Comments.Server.CommentVersion>(commentVersion, nameof (commentVersion));
      List<Guid> vsids = new List<Guid>()
      {
        commentVersion.CreatedBy,
        commentVersion.ModifiedBy
      };
      IDictionary<Guid, IdentityRef> identityRefs = IdentityRefBuilder.Create(requestContext, (IEnumerable<Guid>) vsids, true, true);
      return CommentFactory.Create<T>(requestContext, securedObject, identityRefs, commentVersion);
    }

    private static IEnumerable<Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction> CreateCommentReactions(
      Microsoft.TeamFoundation.Comments.Server.Comment comment)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Comments.Server.Comment>(comment, nameof (comment));
      ArgumentUtility.CheckForNull<IReadOnlyCollection<Microsoft.TeamFoundation.Comments.Server.CommentReaction>>(comment.Reactions, "Reactions");
      return comment.Reactions.Select<Microsoft.TeamFoundation.Comments.Server.CommentReaction, Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction>((Func<Microsoft.TeamFoundation.Comments.Server.CommentReaction, Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction>) (s => CommentReactionFactory.CreateReaction(s, comment.SecuredObject)));
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
        fallbackToConstant = new IdentityRef()
        {
          Id = identityId.ToString(),
          DisplayName = ResourceStrings.UnknownUser()
        };
      return fallbackToConstant;
    }
  }
}
