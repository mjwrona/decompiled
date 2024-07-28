// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.DiscussionThreadsController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  public class DiscussionThreadsController : DiscussionApiController
  {
    [ClientResponseType(typeof (DiscussionThreadCollection), null, null)]
    public List<DiscussionThread> GetThreadsByWorkItemId(int workItemId)
    {
      List<DiscussionComment> comments;
      List<DiscussionThread> threads = this.TfsRequestContext.GetService<TeamFoundationDiscussionService>().QueryDiscussionsByCodeReviewRequest(this.TfsRequestContext, workItemId, out comments);
      this.PopulateThreadsWithComments((IEnumerable<DiscussionThread>) threads, (IList<DiscussionComment>) comments);
      return threads;
    }

    [ClientIgnore]
    public IEnumerable GetThreads([FromUri] string[] artifactUri)
    {
      TeamFoundationDiscussionService service = this.TfsRequestContext.GetService<TeamFoundationDiscussionService>();
      return artifactUri.Length == 1 ? (IEnumerable) service.QueryDiscussionsByArtifactUri(this.TfsRequestContext, artifactUri[0], out List<DiscussionComment> _, new DateTime?(), true) : (IEnumerable) service.QueryDiscussionsByArtifactUris(this.TfsRequestContext, artifactUri);
    }

    [ClientLocationId("010054F6-D9ED-4ED2-855F-7F86BFF10C02")]
    public DiscussionThread GetThread(int discussionId)
    {
      List<DiscussionComment> comments;
      DiscussionThread thread = this.TfsRequestContext.GetService<TeamFoundationDiscussionService>().QueryDiscussionsById(this.TfsRequestContext, discussionId, out comments, true);
      if (thread != null)
        this.PopulateThreadsWithComments((IEnumerable<DiscussionThread>) new DiscussionThread[1]
        {
          thread
        }, (IList<DiscussionComment>) comments);
      return thread;
    }

    [ClientResponseType(typeof (DiscussionThread), null, null)]
    [ClientLocationId("A50DDBE2-1A1D-4C55-857F-73C6A3A31722")]
    [HttpPost]
    public HttpResponseMessage CreateThread(DiscussionThread newThread)
    {
      if (newThread == null)
        throw new ArgumentNullException(nameof (newThread), string.Format(Resources.NewDiscussionPOSTError, (object) this.Request.Content.Headers.ContentLength));
      if (newThread.DiscussionId > 0)
        throw new ArgumentOutOfRangeException("DiscussionId", Resources.InvalidNewDiscussionId);
      this.PublishThreads((IList<DiscussionThread>) new DiscussionThread[1]
      {
        newThread
      });
      return this.Request.CreateResponse<DiscussionThread>(HttpStatusCode.Created, newThread);
    }

    [HttpPatch]
    [ClientResponseType(typeof (DiscussionThreadCollection), null, null)]
    [ClientLocationId("A50DDBE2-1A1D-4C55-857F-73C6A3A31722")]
    public HttpResponseMessage CreateThreads(
      VssJsonCollectionWrapper<DiscussionThreadCollection> newThreads)
    {
      if (newThreads == null)
        throw new HttpResponseException(HttpStatusCode.BadRequest);
      this.PublishThreads((IList<DiscussionThread>) newThreads.Value);
      return this.Request.CreateResponse<IList<DiscussionThread>>(HttpStatusCode.Created, (IList<DiscussionThread>) newThreads.Value);
    }

    private void PublishThreads(IList<DiscussionThread> newThreads)
    {
      if (newThreads == null)
        throw new HttpResponseException(HttpStatusCode.BadRequest);
      TeamFoundationDiscussionService service = this.TfsRequestContext.GetService<TeamFoundationDiscussionService>();
      List<DiscussionComment> comments = new List<DiscussionComment>();
      foreach (DiscussionThread newThread in (IEnumerable<DiscussionThread>) newThreads)
      {
        if (newThread.Comments != null)
        {
          foreach (DiscussionComment comment in newThread.Comments)
          {
            if (comment.DiscussionId != newThread.DiscussionId)
              comment.DiscussionId = comment.DiscussionId == 0 ? newThread.DiscussionId : throw new HttpResponseException(HttpStatusCode.BadRequest);
            comments.Add(comment);
          }
        }
      }
      List<short> commentIds;
      DateTime lastUpdatedDate;
      List<int> intList = service.PublishDiscussions(this.TfsRequestContext, newThreads.ToArray<DiscussionThread>(), comments.ToArray(), (CommentId[]) null, out commentIds, out lastUpdatedDate);
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      for (int index = 0; index < intList.Count; ++index)
      {
        dictionary[newThreads[index].DiscussionId] = intList[index];
        newThreads[index].DiscussionId = intList[index];
        newThreads[index].PublishedDate = lastUpdatedDate;
        newThreads[index].LastUpdatedDate = lastUpdatedDate;
      }
      if (comments.Count != commentIds.Count)
        return;
      for (int index = 0; index < commentIds.Count; ++index)
      {
        comments[index].CommentId = commentIds[index];
        comments[index].PublishedDate = lastUpdatedDate;
        comments[index].LastUpdatedDate = lastUpdatedDate;
        comments[index].LastContentUpdatedDate = lastUpdatedDate;
        comments[index].CanDelete = true;
        comments[index].DiscussionId = dictionary[comments[index].DiscussionId];
      }
      this.PopulateThreadsWithComments((IEnumerable<DiscussionThread>) newThreads, (IList<DiscussionComment>) comments);
    }

    [HttpPatch]
    [ClientLocationId("010054F6-D9ED-4ED2-855F-7F86BFF10C02")]
    [ClientInternalUseOnly(true)]
    public DiscussionThread UpdateThread(int discussionId, DiscussionThread newThread)
    {
      if (newThread == null)
        throw new HttpResponseException(HttpStatusCode.BadRequest);
      TeamFoundationDiscussionService service = this.TfsRequestContext.GetService<TeamFoundationDiscussionService>();
      if (newThread.DiscussionId != discussionId)
        newThread.DiscussionId = newThread.DiscussionId == 0 ? discussionId : throw new HttpResponseException(HttpStatusCode.BadRequest);
      List<DiscussionComment> comments;
      DiscussionThread discussionThread1 = service.QueryDiscussionsById(this.TfsRequestContext, discussionId, out comments, true);
      if (discussionThread1 == null)
        throw new HttpResponseException(HttpStatusCode.NotFound);
      if (!string.IsNullOrEmpty(newThread.ArtifactUri) && !string.Equals(newThread.ArtifactUri, discussionThread1.ArtifactUri, StringComparison.OrdinalIgnoreCase))
        throw new HttpResponseException(HttpStatusCode.BadRequest);
      if (newThread.WorkItemId != 0 && newThread.WorkItemId != discussionThread1.WorkItemId)
        throw new HttpResponseException(HttpStatusCode.BadRequest);
      if (newThread.Properties != null && (discussionThread1.Properties == null && newThread.Properties.Count != 0 || discussionThread1.Properties != null && !newThread.Properties.Equals((object) discussionThread1.Properties)))
        throw new HttpResponseException(HttpStatusCode.BadRequest);
      newThread.IsDirty = true;
      service.PublishDiscussions(this.TfsRequestContext, new DiscussionThread[1]
      {
        newThread
      }, (DiscussionComment[]) null, (CommentId[]) null, out List<short> _, out DateTime _);
      DiscussionThread discussionThread2 = service.QueryDiscussionsById(this.TfsRequestContext, discussionId, out comments, true);
      this.PopulateThreadsWithComments((IEnumerable<DiscussionThread>) new DiscussionThread[1]
      {
        discussionThread2
      }, (IList<DiscussionComment>) comments);
      return discussionThread2;
    }

    private void PopulateThreadsWithComments(
      IEnumerable<DiscussionThread> threads,
      IList<DiscussionComment> comments)
    {
      Dictionary<int, List<DiscussionComment>> dictionary = new Dictionary<int, List<DiscussionComment>>();
      foreach (DiscussionComment comment in (IEnumerable<DiscussionComment>) comments)
      {
        List<DiscussionComment> discussionCommentList = (List<DiscussionComment>) null;
        if (!dictionary.TryGetValue(comment.DiscussionId, out discussionCommentList))
        {
          discussionCommentList = new List<DiscussionComment>();
          dictionary.Add(comment.DiscussionId, discussionCommentList);
        }
        discussionCommentList.Add(comment);
      }
      foreach (DiscussionThread thread in threads)
      {
        List<DiscussionComment> discussionCommentList = (List<DiscussionComment>) null;
        if (dictionary.TryGetValue(thread.DiscussionId, out discussionCommentList))
          thread.Comments = discussionCommentList.ToArray();
      }
      IdentityHelper.PopulateAuthorDisplayNames(this.TfsRequestContext, (IEnumerable<DiscussionComment>) comments);
    }
  }
}
