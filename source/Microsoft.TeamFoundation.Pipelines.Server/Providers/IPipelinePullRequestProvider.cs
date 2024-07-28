// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.IPipelinePullRequestProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public interface IPipelinePullRequestProvider
  {
    ExternalGitPullRequest GetExternalPullRequest(
      IVssRequestContext requestContext,
      string pullRequestUrl,
      JObject authentication);

    ExternalGitPullRequest GetExternalPullRequest(
      IVssRequestContext requestContext,
      JObject authentication,
      string repoName,
      int prNumber,
      int lastCommitsToInclude = 0);

    MergeJobStatus GetMergeJobStatus(ExternalGitPullRequest pullRequest);

    void PostComment(
      IVssRequestContext requestContext,
      JObject authentication,
      string repositoryOwner,
      string repositoryName,
      string issueNumber,
      string message);

    bool DoesUserHaveWritePermissions(
      IVssRequestContext requestContext,
      JObject authentication,
      string userAssociation,
      string repositoryId,
      string userlogin);
  }
}
