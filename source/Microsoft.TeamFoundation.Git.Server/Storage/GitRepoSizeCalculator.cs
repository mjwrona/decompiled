// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Storage.GitRepoSizeCalculator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Storage
{
  internal class GitRepoSizeCalculator
  {
    private readonly IVssRequestContext m_rc;
    private readonly CreateRor m_createRor;

    public GitRepoSizeCalculator(IVssRequestContext rc, CreateRor createRor)
    {
      this.m_rc = rc;
      this.m_createRor = createRor;
    }

    public bool TryComputeAndPersist(
      ITfsGitRepository repo,
      out RepoSizeStats stats,
      out string skippedReason)
    {
      List<TfsGitRef> source = repo.Refs.All();
      Odb odb = GitServerUtils.GetOdb(repo);
      stats = new RepoSizeStats(odb.ContentDB.ObjectCount, odb.ContentDB.Index.PackIds.Count, source.Count, odb.Settings.StablePackfileCapSize);
      if (odb.ReachabilityProvider == null)
      {
        skippedReason = "Skipped size computation because ReachabilityProvider was null (likely an empty ODB).";
        return false;
      }
      foreach (Sha1Id objectId in (IEnumerable<Sha1Id>) this.m_createRor(this.m_rc, odb.ReachabilityProvider).Resolve(repo, (ISet<Sha1Id>) new HashSet<Sha1Id>(), (ISet<Sha1Id>) source.Select<TfsGitRef, Sha1Id>((Func<TfsGitRef, Sha1Id>) (x => x.ObjectId)).ToHashSet<Sha1Id>(), (ICollection<Sha1Id>) Array.Empty<Sha1Id>(), new GitObjectFilter(), false, out ISet<Sha1Id> _, (IObserver<int>) null))
      {
        GitPackObjectType packType;
        TfsGitObjectLocation rawKey;
        odb.ContentDB.LookupObject<TfsGitObjectLocation>(objectId, out packType, out rawKey);
        stats.AddObject(packType, rawKey.Length);
      }
      using (GitCoreComponent gitCoreComponent = this.m_rc.CreateGitCoreComponent())
        gitCoreComponent.UpdateRepoSize(repo.Key, stats.OverallReachableSize);
      skippedReason = (string) null;
      return true;
    }
  }
}
