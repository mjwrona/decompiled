// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CherryPickRelationships
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class CherryPickRelationships : ICherryPickRelationships
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly RepoKey m_repoKey;
    private readonly IGitObjectSet m_objectSet;

    public CherryPickRelationships(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      IGitObjectSet objectSet)
    {
      this.m_requestContext = requestContext;
      this.m_repoKey = repoKey;
      this.m_objectSet = objectSet;
    }

    public List<Sha1Id> GetFamily(Sha1Id commitId)
    {
      this.m_objectSet.LookupObject<TfsGitCommit>(commitId);
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.QueryCherryPickRelationships(this.m_repoKey, commitId);
    }

    public void Add(
      List<(Sha1Id SourceCommitId, Sha1Id TargetCommitId)> sourceToTargetCommitIds)
    {
      HashSet<Sha1Id> sha1IdSet = new HashSet<Sha1Id>();
      foreach ((Sha1Id SourceCommitId, Sha1Id TargetCommitId) toTargetCommitId in sourceToTargetCommitIds)
      {
        if (!sha1IdSet.Add(toTargetCommitId.SourceCommitId))
          throw new ArgumentException("Duplicate Source CommitId.");
      }
      foreach ((Sha1Id SourceCommitId, Sha1Id TargetCommitId) toTargetCommitId in sourceToTargetCommitIds)
      {
        if (sha1IdSet.Contains(toTargetCommitId.TargetCommitId))
          throw new ArgumentException("Target CommitId should not exist in SourceCommmitIds.");
      }
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        gitCoreComponent.AddCherryPickRelationships(this.m_repoKey, sourceToTargetCommitIds);
    }
  }
}
