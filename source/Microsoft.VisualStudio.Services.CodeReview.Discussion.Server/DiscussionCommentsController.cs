// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.DiscussionCommentsController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  public class DiscussionCommentsController : DiscussionApiController
  {
    [ClientResponseType(typeof (DiscussionCommentCollection), null, null)]
    [ClientLocationId("495211BD-B463-4578-86FE-924EA4953693")]
    public List<DiscussionComment> GetComments(int discussionId)
    {
      List<DiscussionComment> comments;
      this.TfsRequestContext.GetService<TeamFoundationDiscussionService>().QueryDiscussionsById(this.TfsRequestContext, discussionId, out comments, true);
      IdentityHelper.PopulateAuthorDisplayNames(this.TfsRequestContext, (IEnumerable<DiscussionComment>) comments);
      return comments;
    }

    [ClientLocationId("495211BD-B463-4578-86FE-924EA4953693")]
    public DiscussionComment GetComment(int discussionId, short commentId)
    {
      List<DiscussionComment> comments;
      this.TfsRequestContext.GetService<TeamFoundationDiscussionService>().QueryDiscussionsById(this.TfsRequestContext, discussionId, out comments, true);
      DiscussionComment comment = comments.Single<DiscussionComment>((Func<DiscussionComment, bool>) (x => (int) x.CommentId == (int) commentId));
      IdentityHelper.PopulateAuthorDisplayNames(this.TfsRequestContext, (IEnumerable<DiscussionComment>) new DiscussionComment[1]
      {
        comment
      });
      return comment;
    }

    [ClientResponseType(typeof (DiscussionComment), null, null)]
    [ClientLocationId("495211BD-B463-4578-86FE-924EA4953693")]
    [HttpPost]
    public HttpResponseMessage AddComment(int discussionId, DiscussionComment newComment)
    {
      this.CreateComments(discussionId, (IList<DiscussionComment>) new DiscussionComment[1]
      {
        newComment
      });
      return this.Request.CreateResponse<DiscussionComment>(HttpStatusCode.Created, newComment);
    }

    [HttpPatch]
    [ClientResponseType(typeof (List<DiscussionComment>), null, null)]
    [ClientLocationId("20933FC0-B6A7-4A57-8111-A7458DA5441B")]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage AddComments(
      [ClientParameterType(typeof (VssJsonCollectionWrapper<IList<DiscussionComment>>), false)] VssJsonCollectionWrapper<DiscussionCommentCollection> newComments)
    {
      TeamFoundationDiscussionService service = this.TfsRequestContext.GetService<TeamFoundationDiscussionService>();
      Dictionary<int, DiscussionThread> dictionary = new Dictionary<int, DiscussionThread>();
      foreach (DiscussionComment discussionComment in (List<DiscussionComment>) newComments.Value)
      {
        if (discussionComment.DiscussionId == 0)
          throw new HttpResponseException(HttpStatusCode.BadRequest);
        if (!dictionary.ContainsKey(discussionComment.DiscussionId))
          dictionary[discussionComment.DiscussionId] = service.QueryDiscussionsById(this.TfsRequestContext, discussionComment.DiscussionId, out List<DiscussionComment> _, true) ?? throw new HttpResponseException(HttpStatusCode.BadRequest);
      }
      List<short> commentIds;
      DateTime lastUpdatedDate;
      service.PublishDiscussions(this.TfsRequestContext, dictionary.Values.ToArray<DiscussionThread>(), newComments.Value.ToArray(), (CommentId[]) null, out commentIds, out lastUpdatedDate);
      if (newComments.Count == commentIds.Count)
      {
        for (int index = 0; index < commentIds.Count; ++index)
        {
          newComments.Value[index].CommentId = commentIds[index];
          newComments.Value[index].LastUpdatedDate = lastUpdatedDate;
          newComments.Value[index].LastContentUpdatedDate = lastUpdatedDate;
          newComments.Value[index].PublishedDate = lastUpdatedDate;
          newComments.Value[index].CanDelete = true;
        }
        IdentityHelper.PopulateAuthorDisplayNames(this.TfsRequestContext, (IEnumerable<DiscussionComment>) newComments.Value);
      }
      return this.Request.CreateResponse<IList<DiscussionComment>>(HttpStatusCode.Created, (IList<DiscussionComment>) newComments.Value);
    }

    [HttpPatch]
    [ClientResponseType(typeof (List<DiscussionComment>), null, null)]
    [ClientLocationId("495211BD-B463-4578-86FE-924EA4953693")]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage AddCommentsByDiscussionId(
      int discussionId,
      [ClientParameterType(typeof (VssJsonCollectionWrapper<IList<DiscussionComment>>), false)] VssJsonCollectionWrapper<DiscussionCommentCollection> newComments)
    {
      this.CreateComments(discussionId, (IList<DiscussionComment>) newComments.Value);
      return this.Request.CreateResponse<IList<DiscussionComment>>(HttpStatusCode.Created, (IList<DiscussionComment>) newComments.Value);
    }

    private void CreateComments(int discussionId, IList<DiscussionComment> newComments)
    {
      TeamFoundationDiscussionService service = this.TfsRequestContext.GetService<TeamFoundationDiscussionService>();
      DiscussionThread discussionThread = service.QueryDiscussionsById(this.TfsRequestContext, discussionId, out List<DiscussionComment> _, true);
      if (discussionThread == null)
        throw new HttpResponseException(HttpStatusCode.NotFound);
      foreach (DiscussionComment newComment in (IEnumerable<DiscussionComment>) newComments)
      {
        if (newComment.DiscussionId != discussionThread.DiscussionId)
          newComment.DiscussionId = newComment.DiscussionId == 0 ? discussionThread.DiscussionId : throw new HttpResponseException(HttpStatusCode.BadRequest);
      }
      List<short> commentIds;
      DateTime lastUpdatedDate;
      service.PublishDiscussions(this.TfsRequestContext, new DiscussionThread[1]
      {
        discussionThread
      }, newComments.ToArray<DiscussionComment>(), (CommentId[]) null, out commentIds, out lastUpdatedDate);
      if (newComments.Count != commentIds.Count)
        return;
      for (int index = 0; index < commentIds.Count; ++index)
      {
        newComments[index].CommentId = commentIds[index];
        newComments[index].LastUpdatedDate = lastUpdatedDate;
        newComments[index].LastContentUpdatedDate = lastUpdatedDate;
        newComments[index].PublishedDate = lastUpdatedDate;
        newComments[index].CanDelete = true;
      }
      IdentityHelper.PopulateAuthorDisplayNames(this.TfsRequestContext, (IEnumerable<DiscussionComment>) newComments);
    }

    [HttpPatch]
    [ClientResponseType(typeof (DiscussionComment), null, null)]
    [ClientLocationId("495211BD-B463-4578-86FE-924EA4953693")]
    public HttpResponseMessage UpdateComment(
      int discussionId,
      short commentId,
      DiscussionComment newComment)
    {
      if (newComment.DiscussionId != discussionId)
        newComment.DiscussionId = newComment.DiscussionId == 0 ? discussionId : throw new HttpResponseException(HttpStatusCode.BadRequest);
      if ((int) newComment.CommentId != (int) commentId)
        newComment.CommentId = newComment.CommentId == (short) 0 ? commentId : throw new HttpResponseException(HttpStatusCode.BadRequest);
      TeamFoundationDiscussionService service = this.TfsRequestContext.GetService<TeamFoundationDiscussionService>();
      try
      {
        ArtifactDiscussionThread discussionThread = new ArtifactDiscussionThread();
        discussionThread.DiscussionId = newComment.DiscussionId;
        DiscussionThread thread = (DiscussionThread) discussionThread;
        DiscussionComment discussionComment = service.UpdateDiscussionComment(this.TfsRequestContext, thread, newComment);
        IdentityHelper.PopulateAuthorDisplayNames(this.TfsRequestContext, (IEnumerable<DiscussionComment>) new DiscussionComment[1]
        {
          discussionComment
        });
        return this.Request.CreateResponse<DiscussionComment>(HttpStatusCode.OK, discussionComment);
      }
      catch (InvalidOperationException ex)
      {
        throw new HttpResponseException(HttpStatusCode.Forbidden);
      }
    }

    [ClientLocationId("495211BD-B463-4578-86FE-924EA4953693")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteComment(int discussionId, short commentId)
    {
      TeamFoundationDiscussionService service = this.TfsRequestContext.GetService<TeamFoundationDiscussionService>();
      List<DiscussionComment> comments;
      DiscussionThread discussionThread = service.QueryDiscussionsById(this.TfsRequestContext, discussionId, out comments, true);
      if (discussionThread == null)
        throw new HttpResponseException(HttpStatusCode.NotFound);
      DiscussionComment comment = comments.FirstOrDefault<DiscussionComment>((Func<DiscussionComment, bool>) (x => (int) x.CommentId == (int) commentId));
      if (comment == null)
        throw new HttpResponseException(HttpStatusCode.NotFound);
      if (!comment.IsDeleted)
      {
        service.PublishDiscussions(this.TfsRequestContext, new DiscussionThread[1]
        {
          discussionThread
        }, (DiscussionComment[]) null, new CommentId[1]
        {
          comment.ToCommentId()
        }, out List<short> _, out DateTime _);
        service.QueryDiscussionsById(this.TfsRequestContext, discussionId, out comments, true);
        if (!comments.First<DiscussionComment>((Func<DiscussionComment, bool>) (x => (int) x.CommentId == (int) commentId)).IsDeleted)
          throw new HttpResponseException(HttpStatusCode.Forbidden);
      }
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }
  }
}
