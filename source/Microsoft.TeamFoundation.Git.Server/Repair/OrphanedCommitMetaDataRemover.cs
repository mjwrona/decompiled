// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Repair.OrphanedCommitMetaDataRemover
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Repair
{
  internal class OrphanedCommitMetaDataRemover
  {
    public static void Execute(
      IVssRequestContext rc,
      Guid repositoryId,
      Sha1Id commitId,
      Action<string> statusReporter = null)
    {
      ITeamFoundationGitRepositoryService service = rc.GetService<ITeamFoundationGitRepositoryService>();
      if (statusReporter != null)
        statusReporter(string.Format("Quering repository {0}.", (object) repositoryId));
      IVssRequestContext requestContext = rc;
      Guid repositoryId1 = repositoryId;
      RepoKey key;
      using (ITfsGitRepository repositoryById = service.FindRepositoryById(requestContext, repositoryId1))
      {
        key = repositoryById.Key;
        ICommitReachabilityProvider reachabilityProvider = GitServerUtils.GetOdb(repositoryById).ReachabilityProvider;
        if (reachabilityProvider == null)
          throw new Exception(string.Format("Repository {0} is either empty or too old.", (object) repositoryId));
        HashSet<Sha1Id> hashSet = repositoryById.Refs.All().Select<TfsGitRef, Sha1Id>((Func<TfsGitRef, Sha1Id>) (x => x.ObjectId)).ToHashSet<Sha1Id>();
        if (statusReporter != null)
          statusReporter(string.Format("Resolving objects reachable from {0} refs in repository {1}. This may take a while...", (object) hashSet.Count, (object) repositoryId));
        ISet<Sha1Id> sha1IdSet = new BitmapReachableObjectResolver(rc, reachabilityProvider).Resolve(repositoryById, (ISet<Sha1Id>) new HashSet<Sha1Id>(), (ISet<Sha1Id>) hashSet, (ICollection<Sha1Id>) new HashSet<Sha1Id>(), new GitObjectFilter(), false, out ISet<Sha1Id> _, (IObserver<int>) null);
        if (statusReporter != null)
          statusReporter(string.Format("Found {0} objects.", (object) sha1IdSet.Count));
        if (sha1IdSet.Contains(commitId))
          throw new Exception(string.Format("Commit {0} is reachable from one of the refs, thus cannot be destroyed from DB.", (object) commitId));
      }
      if (statusReporter != null)
        statusReporter(string.Format("Deleting commit metadata in DB for commit {0} in repository {1}.", (object) commitId, (object) repositoryId));
      using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(key))
        gitOdbComponent.DeleteCommits((IEnumerable<Sha1Id>) new Sha1Id[1]
        {
          commitId
        });
    }
  }
}
