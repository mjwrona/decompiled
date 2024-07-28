// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitCommitTranslator
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public sealed class GitCommitTranslator
  {
    private const int c_shortCommentLength = 100;
    private readonly string m_baseCommitsHref;
    private readonly IVssRequestContext m_rc;

    public GitCommitTranslator(IVssRequestContext rc, RepoKey repoKey, UrlHelper urlHelper = null)
    {
      this.m_rc = rc;
      if (urlHelper != null)
        this.m_baseCommitsHref = urlHelper.RestLink(rc, GitWebApiConstants.CommitsLocationId, RouteValuesFactory.Repo(repoKey)) + "/";
      else
        this.m_baseCommitsHref = rc.GetService<ILocationService>().GetResourceUri(rc, "git", GitWebApiConstants.CommitsLocationId, RouteValuesFactory.Repo(repoKey)).AbsoluteUri + "/";
    }

    public GitCommitRef ToGitCommitShallow(
      TfsGitCommitMetadata metadata,
      bool includeUserImageUrl,
      ISecuredObject securedObject = null)
    {
      string name1;
      string email1;
      GitModelExtensions.ParseNameEmail(metadata.Author, out name1, out email1);
      string name2;
      string email2;
      GitModelExtensions.ParseNameEmail(metadata.Committer, out name2, out email2);
      string commitId = metadata.CommitId.ToString();
      string commitAbsoluteUri = this.GetCommitAbsoluteUri(commitId);
      GitCommitRef gitCommitShallow = new GitCommitRef();
      gitCommitShallow.CommitId = commitId;
      gitCommitShallow.Comment = metadata.ShortComment;
      gitCommitShallow.CommentTruncated = metadata.ShortCommentIsTruncated;
      gitCommitShallow.Author = GitServerUtils.CreateUserDate(this.m_rc, name1, email1, metadata.AuthorTime, includeUserImageUrl);
      gitCommitShallow.Committer = GitServerUtils.CreateUserDate(this.m_rc, name2, email2, metadata.CommitTime, includeUserImageUrl);
      gitCommitShallow.Url = commitAbsoluteUri;
      gitCommitShallow.SetSecuredObject(securedObject);
      return gitCommitShallow;
    }

    public GitCommitRef ToGitCommitShallow(
      TfsGitCommit commit,
      bool includeUserImageUrl,
      int maxCommentLength = 100)
    {
      bool commentTruncated = false;
      string comment = this.GetComment(commit.GetComment(), out commentTruncated, maxCommentLength);
      string commitId = commit.ObjectId.ToString();
      string commitAbsoluteUri = this.GetCommitAbsoluteUri(commitId);
      return new GitCommitRef()
      {
        CommitId = commitId,
        Comment = comment,
        CommentTruncated = commentTruncated,
        Author = GitServerUtils.CreateUserDate(this.m_rc, commit.GetAuthor().Name, commit.GetAuthor().Email, commit.GetAuthor().Time, includeUserImageUrl),
        Committer = GitServerUtils.CreateUserDate(this.m_rc, commit.GetCommitter().Name, commit.GetCommitter().Email, commit.GetCommitter().Time, includeUserImageUrl),
        Url = commitAbsoluteUri
      };
    }

    public GitCommitRef ToGitCommitShallow(
      TfsGitCommitHistoryEntry historyEntry,
      bool includeChangeCounts,
      bool includeUserImageUrl,
      ISecuredObject securedObject = null)
    {
      GitCommitRef gitCommitShallow = this.ToGitCommitShallow(historyEntry.Commit, includeUserImageUrl, securedObject);
      if (includeChangeCounts)
      {
        GitCommitRef gitCommitRef = gitCommitShallow;
        ChangeCounts changeCounts = historyEntry.ChangeCounts;
        ChangeCountDictionary changeCountDictionary = changeCounts != null ? changeCounts.ToChangeCountDictionary() : (ChangeCountDictionary) null;
        gitCommitRef.ChangeCounts = changeCountDictionary;
      }
      return gitCommitShallow;
    }

    public GitCommit ToGitCommit(
      TfsGitCommit tfsGitCommit,
      ISecuredObject securedObject = null,
      bool includeUserImageUrl = false)
    {
      string name1;
      string email1;
      GitModelExtensions.ParseNameEmail(tfsGitCommit.GetAuthor().NameAndEmail, out name1, out email1);
      string name2;
      string email2;
      GitModelExtensions.ParseNameEmail(tfsGitCommit.GetCommitter().NameAndEmail, out name2, out email2);
      string commitId = tfsGitCommit.ObjectId.ToString();
      GitCommit gitCommit = new GitCommit();
      gitCommit.CommitId = commitId;
      gitCommit.Author = GitServerUtils.CreateUserDate(this.m_rc, name1, email1, tfsGitCommit.GetAuthor().Time, includeUserImageUrl);
      gitCommit.Comment = tfsGitCommit.GetComment();
      gitCommit.Committer = GitServerUtils.CreateUserDate(this.m_rc, name2, email2, tfsGitCommit.GetCommitter().Time, includeUserImageUrl);
      gitCommit.Parents = tfsGitCommit.GetParents().Select<TfsGitCommit, string>((Func<TfsGitCommit, string>) (pc => pc.ObjectId.ToString()));
      gitCommit.TreeId = tfsGitCommit.GetTree().ObjectId.ToString();
      gitCommit.Url = this.GetCommitAbsoluteUri(commitId);
      gitCommit.SetSecuredObject(securedObject);
      return gitCommit;
    }

    private string GetComment(string comment, out bool commentTruncated, int maxCommentLength = 100)
    {
      string comment1 = StringUtil.Truncate(comment, maxCommentLength, false);
      commentTruncated = comment1 != comment;
      return comment1;
    }

    private string GetCommitAbsoluteUri(string commitId) => this.m_baseCommitsHref + commitId;
  }
}
