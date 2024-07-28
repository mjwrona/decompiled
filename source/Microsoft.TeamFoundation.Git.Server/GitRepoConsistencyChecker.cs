// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepoConsistencyChecker
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitRepoConsistencyChecker
  {
    private readonly IVssRequestContext m_rc;
    private readonly ITeamFoundationGitRepositoryService m_repoService;
    private readonly ITfsGitRepository m_repo;
    private readonly ClientTraceService m_ctService;
    private readonly IGitKnownFilesProvider m_knownFilesProvider;

    public GitRepoConsistencyChecker(
      IVssRequestContext requestContext,
      ITeamFoundationGitRepositoryService repoService,
      ITfsGitRepository repo,
      ClientTraceService ctService,
      IGitKnownFilesProvider knownFilesProvider)
    {
      this.m_rc = requestContext;
      this.m_repoService = repoService;
      this.m_repo = repo;
      this.m_ctService = ctService;
      this.m_knownFilesProvider = knownFilesProvider;
    }

    public void CheckRepo()
    {
      try
      {
        ClientTraceData properties = this.InitializeCTData("CheckRefConsistency");
        Stopwatch stopwatch = Stopwatch.StartNew();
        foreach (TfsGitRef tfsGitRef in (IEnumerable<TfsGitRef>) this.m_repo.Refs.All())
        {
          if (tfsGitRef.Name.StartsWith("refs/heads/"))
            this.m_repo.LookupObject<TfsGitCommit>(tfsGitRef.ObjectId);
          else
            this.m_repo.LookupObject(tfsGitRef.ObjectId);
        }
        stopwatch.Stop();
        properties.Add("ElapsedTimeMs", (object) stopwatch.ElapsedMilliseconds);
        this.m_ctService.Publish(this.m_rc, "Microsoft.TeamFoundation.Git.Server", "TeamProject", properties);
        this.m_rc.TraceAlways(1013697, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitRepoConsistencyChecker), FormattableString.Invariant(FormattableStringFactory.Create("{0} Repo: {1} passed Ref ", (object) nameof (CheckRepo), (object) this.m_repo.Key.RepoId)) + FormattableString.Invariant(FormattableStringFactory.Create("consistency checker.  Time to complete: {0}", (object) stopwatch.Elapsed)));
        this.m_rc.TraceAlways(1013697, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitRepoConsistencyChecker), FormattableString.Invariant(FormattableStringFactory.Create("Repo: {0} passed CheckRepo.", (object) this.m_repo.Key)));
      }
      catch (Exception ex)
      {
        this.m_rc.TraceAlways(1013697, TraceLevel.Error, GitServerUtils.TraceArea, "GitDataFileProviderService", "Repo: {0} consistency check failed: {1}", (object) this.m_repo.Key.RepoId, (object) ex);
        if (ex is GitFileIsDeletableException)
          this.m_knownFilesProvider.ResetIntervals(this.m_repo.Key.OdbId);
        throw new GitRepoConsistencyCheckerFailedException(this.m_repo.Key, ex);
      }
    }

    private ClientTraceData InitializeCTData(string action)
    {
      ClientTraceData clientTraceData = new ClientTraceData();
      clientTraceData.Add("Action", (object) action);
      clientTraceData.Add("RepositoryId", (object) this.m_repo.Key.RepoId);
      clientTraceData.Add("RepositoryName", (object) this.m_repo.Name);
      return clientTraceData;
    }
  }
}
