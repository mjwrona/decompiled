// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPullRequestConflictService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitPullRequestConflictService : GitConflictService
  {
    protected override void NotifySourceThatConflictWasResolved(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int conflictSourceId)
    {
      TeamFoundationGitPullRequestService service = (TeamFoundationGitPullRequestService) requestContext.GetService<ITeamFoundationGitPullRequestService>();
      if (service.GetPullRequestDetails(requestContext, repository, conflictSourceId).Status != PullRequestStatus.Active || GitConflictService.QueryGitConflicts(requestContext, repository, GitConflictSourceType.PullRequest, conflictSourceId, 1, excludeResolved: true).Count != 0)
        return;
      service.EnterMergeJob(requestContext, repository, conflictSourceId);
    }
  }
}
