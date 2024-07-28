// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.Web.Controllers.CommentsBaseController
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.Azure.DevOps.Comments.Web.Common;
using Microsoft.Azure.DevOps.Comments.Web.Factories;
using Microsoft.Azure.DevOps.Comments.WebApi;
using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.Azure.DevOps.Comments.Web.Controllers
{
  [ApiTelemetry(true, false)]
  public abstract class CommentsBaseController : TfsProjectApiController
  {
    protected virtual ICommentService CommentService => this.TfsRequestContext.GetService<ICommentService>();

    protected virtual T GetComment<TList, T>(
      Guid artifactKind,
      string artifactId,
      int commentId,
      bool? excludeDeleted = null,
      CommentExpandOptions expandOptions = CommentExpandOptions.None)
      where TList : CommentList, new()
      where T : Microsoft.Azure.DevOps.Comments.WebApi.Comment, new()
    {
      ValidationHelper.ValidateCommentId(commentId);
      T comment = CommentFactory.Create<TList, T>(this.TfsRequestContext, this.CommentService.GetComment(this.TfsRequestContext, this.ProjectId, artifactKind, artifactId, commentId, (ExpandOptions) expandOptions, !excludeDeleted.GetValueOrDefault()));
      this.AddUrlToComment((Microsoft.Azure.DevOps.Comments.WebApi.Comment) comment);
      return comment;
    }

    protected virtual TList GetComments<TList, T>(
      Guid artifactKind,
      string artifactId,
      int? top = null,
      string continuationToken = null,
      bool? excludeDeleted = null,
      CommentExpandOptions expandOptions = CommentExpandOptions.None,
      CommentSortOrder? order = null,
      int? parentId = null)
      where TList : CommentList, new()
      where T : Microsoft.Azure.DevOps.Comments.WebApi.Comment, new()
    {
      ValidationHelper.ValidateTop(top);
      TList comments = CommentListFactory.Create<TList, T>(this.TfsRequestContext, this.CommentService.GetComments(this.TfsRequestContext, this.ProjectId, artifactKind, artifactId, top, continuationToken, (ExpandOptions) expandOptions, !excludeDeleted.GetValueOrDefault(), order.HasValue ? (Microsoft.TeamFoundation.Comments.Server.SortOrder) order.Value : Microsoft.TeamFoundation.Comments.Server.SortOrder.Desc, parentId));
      this.AddUrlToComments((CommentList) comments);
      return comments;
    }

    protected virtual TList GetCommentsBatch<TList, T>(
      Guid artifactKind,
      string artifactId,
      string commentIds,
      bool? excludeDeleted = null,
      CommentExpandOptions expandOptions = CommentExpandOptions.None)
      where TList : CommentList, new()
      where T : Microsoft.Azure.DevOps.Comments.WebApi.Comment, new()
    {
      ISet<int> intSet = (ISet<int>) new HashSet<int>(ParsingHelper.ParseIds(commentIds).Distinct<int>());
      int count = intSet.Count;
      if (count < 1 || count > Microsoft.TeamFoundation.Comments.Server.CommentService.MaxAllowedPageSize)
        throw new VssPropertyValidationException("commentIds.length", ResourceStrings.QueryParameterOutOfRangeWithRangeValues((object) "commentIds.length", (object) 1, (object) Microsoft.TeamFoundation.Comments.Server.CommentService.MaxAllowedPageSize));
      IEnumerable<int> ints = intSet.Where<int>((Func<int, bool>) (cid => cid < 1));
      if (ints.Any<int>())
        throw new VssPropertyValidationException("ids", ResourceStrings.QueryParameterOutOfRange((object) string.Join<int>(",", ints)));
      TList comments = CommentListFactory.Create<TList, T>(this.TfsRequestContext, this.CommentService.GetComments(this.TfsRequestContext, this.ProjectId, artifactKind, artifactId, intSet, (ExpandOptions) expandOptions, !excludeDeleted.GetValueOrDefault()));
      this.AddUrlToComments((CommentList) comments);
      return comments;
    }

    protected virtual T AddComment<TList, T>(
      Guid artifactKind,
      string artifactId,
      CommentCreateParameters comment)
      where TList : CommentList, new()
      where T : Microsoft.Azure.DevOps.Comments.WebApi.Comment, new()
    {
      if (string.IsNullOrWhiteSpace(comment?.Text))
        throw new VssPropertyValidationException("text", ResourceStrings.NullOrEmptyParameter((object) "text"));
      Microsoft.TeamFoundation.Comments.Server.AddComment comment1 = new Microsoft.TeamFoundation.Comments.Server.AddComment(artifactId, comment.Text)
      {
        ParentId = comment.ParentId
      };
      T obj = CommentFactory.Create<TList, T>(this.TfsRequestContext, this.CommentService.AddComment(this.TfsRequestContext, this.ProjectId, artifactKind, comment1));
      this.AddUrlToComment((Microsoft.Azure.DevOps.Comments.WebApi.Comment) obj);
      return obj;
    }

    protected virtual T UpdateComment<TList, T>(
      Guid artifactKind,
      string artifactId,
      int commentId,
      CommentUpdateParameters comment)
      where TList : CommentList, new()
      where T : Microsoft.Azure.DevOps.Comments.WebApi.Comment, new()
    {
      ValidationHelper.ValidateCommentId(commentId);
      Microsoft.TeamFoundation.Comments.Server.UpdateComment comment1 = new Microsoft.TeamFoundation.Comments.Server.UpdateComment(artifactId, commentId, comment.Text, new Microsoft.TeamFoundation.Comments.Server.CommentState?((Microsoft.TeamFoundation.Comments.Server.CommentState) comment.State));
      T obj = CommentFactory.Create<TList, T>(this.TfsRequestContext, this.CommentService.UpdateComment(this.TfsRequestContext, this.ProjectId, artifactKind, comment1));
      this.AddUrlToComment((Microsoft.Azure.DevOps.Comments.WebApi.Comment) obj);
      return obj;
    }

    protected virtual HttpResponseMessage DeleteComment(
      Guid artifactKind,
      string artifactId,
      int commentId)
    {
      ValidationHelper.ValidateCommentId(commentId);
      this.CommentService.DeleteComment(this.TfsRequestContext, this.ProjectId, artifactKind, artifactId, commentId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    protected abstract void AddUrlToComment(Microsoft.Azure.DevOps.Comments.WebApi.Comment comment);

    protected abstract void AddUrlToComments(CommentList comments);
  }
}
