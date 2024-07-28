// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitReferenceLinksUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitReferenceLinksUtility
  {
    public static ReferenceLinks GetBaseReferenceLinks(
      IVssRequestContext rc,
      string selfLink,
      RepoKey repoKey,
      ISecuredObject securedObject = null)
    {
      return GitReferenceLinksUtility.GetBaseReferenceLinks(selfLink, GitReferenceLinksUtility.GetRepositoryUrl(rc, repoKey), securedObject);
    }

    public static ReferenceLinks GetBaseReferenceLinks(
      string selfLink,
      string repositoryUrl,
      ISecuredObject securedObject = null)
    {
      ReferenceLinks baseReferenceLinks = new ReferenceLinks();
      baseReferenceLinks.AddLink("self", selfLink, securedObject);
      baseReferenceLinks.AddLink("repository", repositoryUrl, securedObject);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetBlobReferenceLinks(
      this GitBlobRef gitBlob,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      ISecuredObject securedObject)
    {
      return GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, gitBlob.Url, repoKey, securedObject);
    }

    public static ReferenceLinks GetTreeReferenceLinks(
      this GitTreeRef gitTree,
      IVssRequestContext requestContext,
      RepoKey repoKey)
    {
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, gitTree.Url, repoKey);
      if (gitTree.TreeEntries != null)
      {
        foreach (GitTreeEntryRef treeEntry in gitTree.TreeEntries)
        {
          if (treeEntry.Url != null)
            baseReferenceLinks.AddLink("treeEntries", treeEntry.Url);
        }
      }
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetCommitReferenceLinks(
      this GitCommitRef gitCommitRef,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      ISecuredObject securedObject = null)
    {
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, gitCommitRef.Url, repoKey, securedObject);
      if (gitCommitRef.RemoteUrl != null)
        baseReferenceLinks.AddLink("web", gitCommitRef.RemoteUrl, securedObject);
      baseReferenceLinks.AddLink("changes", requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.CommitChangesLocationId, RouteValuesFactory.Commit(repoKey, gitCommitRef.CommitId)).AbsoluteUri, securedObject);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetCommitReferenceLinks(
      this GitCommit gitCommit,
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      RepoKey repoKey)
    {
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, gitCommit.Url, repoKey);
      baseReferenceLinks.AddLink("changes", urlHelper.RestLink(requestContext, GitWebApiConstants.CommitChangesLocationId, RouteValuesFactory.Commit(repoKey, gitCommit.CommitId)));
      if (gitCommit.RemoteUrl != null)
        baseReferenceLinks.AddLink("web", gitCommit.RemoteUrl);
      if (gitCommit.Push?.Url != null)
        baseReferenceLinks.AddLink("pushes", gitCommit.Push.Url);
      baseReferenceLinks.AddLink("tree", urlHelper.RestLink(requestContext, GitWebApiConstants.TreesLocationId, RouteValuesFactory.TreeOrBlob(repoKey, gitCommit.TreeId)));
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetItemReferenceLinks(
      this GitItem gitItem,
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      RepoKey repoKey)
    {
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, gitItem.Url, repoKey, (ISecuredObject) gitItem);
      if (gitItem.IsFolder)
        baseReferenceLinks.AddLink("tree", urlHelper.RestLink(requestContext, GitWebApiConstants.TreesLocationId, RouteValuesFactory.TreeOrBlob(repoKey, gitItem.ObjectId)), (ISecuredObject) gitItem);
      else
        baseReferenceLinks.AddLink("blob", urlHelper.RestLink(requestContext, GitWebApiConstants.BlobsLocationId, RouteValuesFactory.TreeOrBlob(repoKey, gitItem.ObjectId)), (ISecuredObject) gitItem);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetPushReferenceLinks(
      this GitPush gitPush,
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      RepoKey repoKey)
    {
      return gitPush.GetPushReferenceLinks(requestContext, urlHelper, repoKey, (ISecuredObject) null);
    }

    public static ReferenceLinks GetPushReferenceLinks(
      this GitPush gitPush,
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      RepoKey repoKey,
      ISecuredObject securedObject)
    {
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, gitPush.Url, repoKey, securedObject);
      baseReferenceLinks.AddLink("commits", GitReferenceLinksUtility.GetPushCommitsUrl(requestContext, repoKey.RepoId, gitPush.PushId), securedObject);
      if (gitPush.PushedBy != null)
        baseReferenceLinks.AddLink("pusher", gitPush.PushedBy.Url, securedObject);
      if (gitPush.RefUpdates != null)
      {
        foreach (GitRefUpdate refUpdate in gitPush.RefUpdates)
          baseReferenceLinks.AddLink("refs", GitReferenceLinksUtility.GetRefUrl(requestContext, repoKey, refUpdate.Name, urlHelper), securedObject);
      }
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetCherryPickOperationReferenceLinks(
      this GitCherryPick operation,
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      RepoKey repoKey)
    {
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, operation.Url, repoKey);
      if (operation.Parameters.Source.PullRequestId.HasValue)
      {
        int pullRequestId = operation.Parameters.Source.PullRequestId.Value;
        baseReferenceLinks.AddLink("pullRequests", urlHelper.RestLink(requestContext, GitWebApiConstants.PullRequestsLocationId, RouteValuesFactory.PullRequest(repoKey, pullRequestId)));
      }
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetRevertOperationReferenceLinks(
      this GitRevert operation,
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      RepoKey repoKey)
    {
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, operation.Url, repoKey);
      if (operation.Parameters.Source.PullRequestId.HasValue)
      {
        int pullRequestId = operation.Parameters.Source.PullRequestId.Value;
        baseReferenceLinks.AddLink("pullRequests", urlHelper.RestLink(requestContext, GitWebApiConstants.PullRequestsLocationId, RouteValuesFactory.PullRequest(repoKey, pullRequestId)));
      }
      return baseReferenceLinks;
    }

    public static string BuildRefUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string refName)
    {
      return GitReferenceLinksUtility.BuildRefUrl(GitReferenceLinksUtility.GetRefsUri(requestContext, repoKey), refName);
    }

    public static string BuildRefUrl(Uri baseRefsUri, string refName) => baseRefsUri.AppendQuery("filter", refName.Substring("refs/".Length)).ToString();

    public static ReferenceLinks GetRepositoryReferenceLinks(
      this GitRepository gitRepository,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      ReferenceLinks repositoryReferenceLinks = new ReferenceLinks();
      repositoryReferenceLinks.AddLink("self", gitRepository.Url, securedObject);
      repositoryReferenceLinks.AddLink("project", gitRepository.GetTeamProjectUri(), securedObject);
      repositoryReferenceLinks.AddLink("web", gitRepository.WebUrl, securedObject);
      if (gitRepository.SshUrl != null)
        repositoryReferenceLinks.AddLink("ssh", gitRepository.SshUrl, securedObject);
      ILocationService service = requestContext.GetService<ILocationService>();
      object routeValues = RouteValuesFactory.Repo(new RepoKey(gitRepository.ProjectReference.Id, gitRepository.Id));
      repositoryReferenceLinks.AddLink("commits", service.GetResourceUri(requestContext, "git", GitWebApiConstants.CommitsLocationId, routeValues).AbsoluteUri, securedObject);
      repositoryReferenceLinks.AddLink("refs", service.GetResourceUri(requestContext, "git", GitWebApiConstants.RefsLocationId, routeValues).AbsoluteUri, securedObject);
      repositoryReferenceLinks.AddLink("pullRequests", service.GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestsLocationId, routeValues).AbsoluteUri, securedObject);
      repositoryReferenceLinks.AddLink("items", service.GetResourceUri(requestContext, "git", GitWebApiConstants.ItemsLocationId, routeValues).AbsoluteUri, securedObject);
      repositoryReferenceLinks.AddLink("pushes", service.GetResourceUri(requestContext, "git", GitWebApiConstants.PushesLocationId, routeValues).AbsoluteUri, securedObject);
      return repositoryReferenceLinks;
    }

    public static string GetRepositoryUrl(IVssRequestContext requestContext, RepoKey repoKey) => requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.RepositoriesLocationId, RouteValuesFactory.Repo(repoKey)).AbsoluteUri;

    public static string GetCommitRefUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string commitId,
      UrlHelper urlHelper)
    {
      return urlHelper != null ? urlHelper.RestLink(requestContext, GitWebApiConstants.CommitsLocationId, RouteValuesFactory.Commit(repoKey, commitId)) : requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.CommitsLocationId, RouteValuesFactory.Commit(repoKey, commitId)).AbsoluteUri;
    }

    public static string GetBlobRefUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string blobId,
      UrlHelper urlHelper)
    {
      return urlHelper != null ? urlHelper.RestLink(requestContext, GitWebApiConstants.BlobsLocationId, RouteValuesFactory.TreeOrBlob(repoKey, blobId)) : requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.BlobsLocationId, RouteValuesFactory.TreeOrBlob(repoKey, blobId)).AbsoluteUri;
    }

    public static string GetTreeRefUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string treeId,
      UrlHelper urlHelper)
    {
      return urlHelper != null ? urlHelper.RestLink(requestContext, GitWebApiConstants.TreesLocationId, RouteValuesFactory.TreeOrBlob(repoKey, treeId)) : requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.TreesLocationId, RouteValuesFactory.TreeOrBlob(repoKey, treeId)).AbsoluteUri;
    }

    public static string GetRefUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string refName)
    {
      return GitReferenceLinksUtility.GetRefUrl(requestContext, repoKey, refName, (UrlHelper) null);
    }

    public static string GetRefUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string refName,
      UrlHelper urlHelper)
    {
      if (refName != null && refName.StartsWith("refs/", StringComparison.Ordinal))
        refName = refName.Substring("refs/".Length);
      return urlHelper != null ? urlHelper.RestLink(requestContext, GitWebApiConstants.RefsLocationId, RouteValuesFactory.Ref(repoKey, refName)) : requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.RefsLocationId, RouteValuesFactory.Ref(repoKey, refName)).AbsoluteUri;
    }

    public static Uri GetRefsUri(IVssRequestContext requestContext, RepoKey repoKey) => requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.RefsLocationId, RouteValuesFactory.Repo(repoKey));

    public static string GetPushCommitsUrl(
      IVssRequestContext requestContext,
      Guid repositoryId,
      int pushId)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.PushCommitsLocationId, (object) new
      {
        repositoryId = repositoryId,
        pushId = pushId,
        resource = "commits"
      }).AbsoluteUri;
    }
  }
}
