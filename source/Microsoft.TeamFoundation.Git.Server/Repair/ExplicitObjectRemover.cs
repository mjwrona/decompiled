// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Repair.ExplicitObjectRemover
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Storage;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Repair
{
  internal class ExplicitObjectRemover
  {
    private const string c_layer = "ExplicitObjectRemover";

    public static void Execute(
      IVssRequestContext rc,
      ITFLogger log,
      OdbId odbId,
      IEnumerable<Sha1Id> objectIds)
    {
      HashSet<Sha1Id> objectsToRemove = new HashSet<Sha1Id>(objectIds);
      if (rc.IsFeatureEnabled("Git.IsolationBitmap.Write"))
        throw new NotImplementedException("ExplicitObjectRemover cannot handle race conditions when Git.IsolationBitmap.Write is enabled.");
      foreach (TfsGitRepositoryInfo gitRepositoryInfo in rc.GetService<IInternalGitRepoService>().QueryRepositoriesByOdbId(rc, odbId))
      {
        using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        {
          Guid? oldPointerId = gitCoreComponent.ReadPointer(gitRepositoryInfo.Key, RepoPointerType.OdbIsolationBitmap);
          if (oldPointerId.HasValue)
          {
            Guid? nullable = gitCoreComponent.UpdatePointer(gitRepositoryInfo.Key, RepoPointerType.OdbIsolationBitmap, oldPointerId, new Guid?());
            if (nullable.HasValue)
              throw new InvalidOperationException(string.Format("Deleting the {0} pointer failed because it was concurrently updated from {1} to {2}", (object) RepoPointerType.OdbIsolationBitmap, (object) oldPointerId, (object) nullable));
            log.Info(string.Format("Deleted {0} pointer previously set to {1}", (object) RepoPointerType.OdbIsolationBitmap, (object) oldPointerId));
          }
        }
      }
      RepairUtils.OdbQuiesce(rc, odbId, log);
      using (Odb odb = DefaultGitDependencyRoot.Instance.CreateOdb(rc, odbId))
      {
        GitPackIndexTransaction indexTransaction = odb.PackIndexTranFactory();
        try
        {
          indexTransaction.EnsureIndexLease();
          Sha1Id? index = odb.PackIndexPointerProvider.GetIndex();
          using (ConcatGitPackIndex concatGitPackIndex = odb.PackIndexLoader.LoadIndex(index))
          {
            GitPackIndexer indexer = new GitPackIndexer();
            indexer.SetBaseIndex(ConcatGitPackIndex.Empty);
            indexer.AddFromIndex(concatGitPackIndex, (Predicate<Sha1Id>) (x => !objectsToRemove.Contains(x)));
            indexer.StartStableObjectOrderEpoch(StorageUtils.CreateUniqueId());
            Sha1Id sha1Id = GitDataFileUtil.WriteIndex(rc, odb.BlobProvider, odb.Id, indexTransaction.KnownFilesBuilder, indexer, PackIndexMergeStrategy.ForceFull);
            using (ConcatGitPackIndex newIndex = odb.PackIndexLoader.LoadIndex(new Sha1Id?(sha1Id)))
              indexTransaction.CommitAndDispose(concatGitPackIndex, newIndex, removedObjects: (IEnumerable<Sha1Id>) objectsToRemove);
            log.Info(string.Format("Updated {0} pointer from {1} to {2}", (object) OdbPointerType.Index, (object) index, (object) sha1Id));
          }
        }
        finally
        {
          indexTransaction.TryExpirePendingExtantAndDispose();
        }
      }
      RepairUtils.OdbUnquiesce(rc, odbId, log);
    }
  }
}
