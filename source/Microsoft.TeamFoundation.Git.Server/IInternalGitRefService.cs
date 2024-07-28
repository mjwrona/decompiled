// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IInternalGitRefService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Policy;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationGitRefService))]
  internal interface IInternalGitRefService : IVssFrameworkService
  {
    TfsGitRefUpdateResultSet UpdateRefs(
      IVssRequestContext rc,
      Guid repoId,
      List<TfsGitRefUpdateRequest> refUpdates,
      List<TfsIncludedGitCommit> includedCommits = null,
      GitRefUpdateMode updateMode = GitRefUpdateMode.BestEffort,
      IGitPushReporter pushReporter = null,
      ClientTraceData ctData = null,
      ITeamFoundationGitRefUpdateValidator refUpdateValidator = null,
      QueuedGitPushJobsContext queuedGitPushJobsContext = null);

    Guid ReadRefCreatorWithDefaultAce(IVssRequestContext rc, RepoKey repoKey, string refName);

    string ReadAnyRefWithDefaultAceByCreator(
      IVssRequestContext rc,
      RepoKey repoKey,
      Guid creatorId);

    void SetAccessControlListForRefCreator(IVssRequestContext rc, string token);
  }
}
