// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Utils.DefaultBranchUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Utils
{
  public static class DefaultBranchUtils
  {
    public static bool IsDefaultBranch(
      IVssRequestContext requestContext,
      Guid repoId,
      string refName)
    {
      return DefaultBranchUtils.IsAnyDefaultBranch(requestContext, repoId, new string[1]
      {
        refName
      });
    }

    public static bool IsAnyDefaultBranch(
      IVssRequestContext requestContext,
      Guid repoId,
      string[] refNames)
    {
      if (refNames == null || refNames.Length == 0 || !((IEnumerable<string>) refNames).Any<string>((Func<string, bool>) (refName => !string.IsNullOrEmpty(refName))))
        return false;
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repository = service.FindRepositoryById(requestContext, repoId))
        return ((IEnumerable<string>) refNames).Any<string>((Func<string, bool>) (refName => !string.IsNullOrEmpty(refName) && refName.Equals(repository.Refs.GetDefault()?.Name)));
    }

    public static bool IsPullRequestTargetDefaultBranch(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      return DefaultBranchUtils.IsDefaultBranch(requestContext, pullRequest.RepositoryId, pullRequest.TargetBranchName);
    }

    public static bool IsPullRequestSourceDefaultBranch(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      return DefaultBranchUtils.IsDefaultBranch(requestContext, pullRequest.RepositoryId, pullRequest.SourceBranchName);
    }
  }
}
