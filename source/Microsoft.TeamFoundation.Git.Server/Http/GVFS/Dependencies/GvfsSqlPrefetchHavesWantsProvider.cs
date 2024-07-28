// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Http.GVFS.Dependencies.GvfsSqlPrefetchHavesWantsProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Http.GVFS.Dependencies
{
  internal class GvfsSqlPrefetchHavesWantsProvider : IGvfsPrefetchHavesWantsProvider
  {
    private readonly IVssRequestContext m_rc;
    private readonly ITfsGitRepository m_repo;

    public GvfsSqlPrefetchHavesWantsProvider(IVssRequestContext rc, ITfsGitRepository repo)
    {
      this.m_rc = rc;
      this.m_repo = repo;
    }

    public (HashSet<Sha1Id> haves, HashSet<Sha1Id> wants) ReadHavesWantsSince(long timestamp)
    {
      DateTime timestamp1 = GitServerConstants.UtcEpoch.AddSeconds((double) timestamp);
      using (GitCoreComponent gitCoreComponent = this.m_rc.CreateGitCoreComponent())
        return gitCoreComponent.GetPrefetchHavesWants(this.m_repo.Key, timestamp1);
    }
  }
}
