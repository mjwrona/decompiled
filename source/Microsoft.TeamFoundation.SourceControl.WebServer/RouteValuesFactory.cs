// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.RouteValuesFactory
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Git.Server;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal static class RouteValuesFactory
  {
    public static object Repo(RepoKey repoKey) => (object) new
    {
      project = repoKey.ProjectId,
      repositoryId = repoKey.RepoId
    };

    public static object Commit(RepoKey repoKey, string commitId) => (object) new
    {
      project = repoKey.ProjectId,
      repositoryId = repoKey.RepoId,
      commitId = commitId
    };

    public static object Ref(RepoKey repoKey, string refName) => (object) new
    {
      project = repoKey.ProjectId,
      repositoryId = repoKey.RepoId,
      filter = refName
    };

    public static object Iteration(RepoKey repoKey, int pullRequestId, int iterationId) => (object) new
    {
      project = repoKey.ProjectId,
      repositoryId = repoKey.RepoId,
      pullRequestId = pullRequestId,
      iterationId = iterationId
    };

    public static object PullRequest(RepoKey repoKey, int pullRequestId) => (object) new
    {
      project = repoKey.ProjectId,
      repositoryId = repoKey.RepoId,
      pullRequestId = pullRequestId
    };

    public static object PRReviewer(RepoKey repoKey, int pullRequestId, string reviewerId) => (object) new
    {
      project = repoKey.ProjectId,
      repositoryId = repoKey.RepoId,
      pullRequestId = pullRequestId,
      reviewerId = reviewerId
    };

    public static object PRConflict(RepoKey repoKey, int pullRequestId, int conflictId) => (object) new
    {
      project = repoKey.ProjectId,
      repositoryId = repoKey.RepoId,
      pullRequestId = pullRequestId,
      conflictId = conflictId
    };

    public static object TreeOrBlob(RepoKey repoKey, string sha1Id) => (object) new
    {
      project = repoKey.ProjectId,
      repositoryId = repoKey.RepoId,
      sha1 = sha1Id
    };

    public static object AnnotatedTag(RepoKey repoKey, Sha1Id objectId) => (object) new
    {
      project = repoKey.ProjectId,
      repositoryId = repoKey.RepoId,
      objectId = objectId
    };
  }
}
