// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestDiscussionConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class PullRequestDiscussionConverter
  {
    public static GitPullRequestCommentThread ToGitPullRequestItem(
      this DiscussionThread discussionThread,
      IVssRequestContext rc,
      ISecuredObject securedObject,
      IDictionary<Guid, IdentityRef> cachedIdentities = null)
    {
      if (discussionThread == null)
        return (GitPullRequestCommentThread) null;
      cachedIdentities = cachedIdentities ?? (IDictionary<Guid, IdentityRef>) new Dictionary<Guid, IdentityRef>();
      GitPullRequestCommentThread gitPullRequestItem = new GitPullRequestCommentThread();
      gitPullRequestItem.Id = discussionThread.DiscussionId;
      gitPullRequestItem.ArtifactUri = discussionThread.ArtifactUri;
      gitPullRequestItem.LastUpdatedDate = discussionThread.LastUpdatedDate;
      gitPullRequestItem.PublishedDate = discussionThread.PublishedDate;
      Dictionary<string, IdentityRef> identities;
      gitPullRequestItem.Properties = ThreadPropertiesConverter.GetPRThreadProperties(rc, discussionThread, cachedIdentities, securedObject, out identities);
      gitPullRequestItem.Identities = identities;
      gitPullRequestItem.Status = discussionThread.Status.ToCommentStatus();
      gitPullRequestItem.Comments = ((IEnumerable<DiscussionComment>) discussionThread.Comments).ToCommentList();
      gitPullRequestItem.ThreadContext = discussionThread.ToThreadContext();
      gitPullRequestItem.PullRequestThreadContext = discussionThread.ToPRThreadContext();
      gitPullRequestItem.IsDeleted = discussionThread.IsDeleted;
      gitPullRequestItem.MarkForUpdate = discussionThread.IsDirty;
      gitPullRequestItem.SetSecuredObject(securedObject);
      return gitPullRequestItem;
    }

    public static GitPullRequestCommentThreadContext ToPRThreadContext(
      this DiscussionThread discussionThread)
    {
      if (discussionThread == null)
        return (GitPullRequestCommentThreadContext) null;
      return new GitPullRequestCommentThreadContext()
      {
        IterationContext = new CommentIterationContext()
        {
          FirstComparingIteration = 1,
          SecondComparingIteration = 1
        }
      };
    }
  }
}
