// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.ICommentService
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Comments.Server
{
  [DefaultServiceImplementation(typeof (CommentService))]
  public interface ICommentService : IVssFrameworkService
  {
    CommentsList GetComments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      ISet<int> ids,
      ExpandOptions expandOptions = ExpandOptions.None,
      bool includeDeleted = false);

    CommentsList GetComments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int? top = null,
      string continuationToken = null,
      ExpandOptions expandOptions = ExpandOptions.None,
      bool includeDeleted = false,
      SortOrder order = SortOrder.Desc,
      int? parentId = null);

    Comment GetComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      ExpandOptions expandOptions = ExpandOptions.None,
      bool includeDeleted = false);

    IList<CommentVersion> GetCommentVersions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId);

    CommentVersion GetCommentVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      int version);

    Comment AddComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      Microsoft.TeamFoundation.Comments.Server.AddComment comment);

    Comment UpdateComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      Microsoft.TeamFoundation.Comments.Server.UpdateComment comment);

    Comment DeleteComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId);

    Comment DeleteComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      Microsoft.TeamFoundation.Comments.Server.DeleteComment comment);

    Comment ProcessComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      Microsoft.TeamFoundation.Comments.Server.ProcessComment comment);

    void DestroyCommentsForArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      ISet<string> artifactIds);
  }
}
