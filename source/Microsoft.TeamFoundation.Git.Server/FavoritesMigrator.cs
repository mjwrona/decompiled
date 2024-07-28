// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.FavoritesMigrator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class FavoritesMigrator
  {
    private readonly IVssRequestContext m_rc;

    public FavoritesMigrator(IVssRequestContext rc) => this.m_rc = rc;

    public string Migrate(
      Guid projectId,
      ITfsGitRepository sourceRepo,
      ITfsGitRepository targetRepo)
    {
      using (GitCoreComponent gitCoreComponent = this.m_rc.CreateGitCoreComponent())
        return string.Format("Copied {0} favorites.", (object) gitCoreComponent.CopyRefFavorites(sourceRepo.Key, targetRepo.Key));
    }
  }
}
