// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITeamFoundationGitCommitStatusService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationGitCommitStatusService))]
  public interface ITeamFoundationGitCommitStatusService : IVssFrameworkService
  {
    ILookup<Sha1Id, GitStatus> GetStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<Sha1Id> commits,
      int top,
      int skip,
      bool latestOnly = false);

    GitStatus AddStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId,
      GitStatus gitCommitStatusToCreate);
  }
}
