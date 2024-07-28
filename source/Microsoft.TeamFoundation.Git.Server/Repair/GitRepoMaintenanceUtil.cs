// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Repair.GitRepoMaintenanceUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Git.Server.Repair
{
  internal class GitRepoMaintenanceUtil
  {
    public static void SetGitRepoMaintenanceFlag(IVssRequestContext rc, Guid repoId, ITFLogger log)
    {
      ITeamFoundationGitRepositoryService service = rc.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repositoryById = service.FindRepositoryById(rc, repoId))
      {
        log.Info(string.Format("Setting maintenance flag for repo {0}", (object) repoId));
        service.SetGitRepoMaintenanceFlagByObdId(rc, repositoryById.Key.OdbId.Value, true);
      }
    }

    public static void ClearGitRepoMaintenanceFlag(
      IVssRequestContext rc,
      Guid repoId,
      ITFLogger log)
    {
      ITeamFoundationGitRepositoryService service = rc.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repositoryById = service.FindRepositoryById(rc, repoId))
      {
        log.Info(string.Format("Clearing maintenance flag for repo {0}", (object) repoId));
        service.SetGitRepoMaintenanceFlagByObdId(rc, repositoryById.Key.OdbId.Value, false);
      }
    }
  }
}
