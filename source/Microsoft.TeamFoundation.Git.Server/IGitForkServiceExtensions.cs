// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitForkServiceExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Forks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class IGitForkServiceExtensions
  {
    public static ITfsGitRepository CreateAndSyncFork(
      this IGitForkService forkSvc,
      IVssRequestContext rc,
      GitForkSyncRequestParameters sourceParams,
      Guid targetProjectId,
      string newRepositoryName,
      out ForkFetchAsyncOp fetchOp)
    {
      ITfsGitRepository fork = forkSvc.CreateFork(rc, sourceParams.Source, targetProjectId, newRepositoryName);
      bool flag = true;
      try
      {
        fetchOp = forkSvc.SyncFork(rc, sourceParams, fork.Key, true);
        flag = false;
      }
      finally
      {
        if (flag && fork != null)
          fork.Dispose();
      }
      return fork;
    }
  }
}
