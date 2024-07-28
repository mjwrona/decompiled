// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.BasicReachabilityProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class BasicReachabilityProvider : ICommitReachabilityProvider
  {
    private readonly IVssRequestContext m_rc;
    private readonly IGitObjectSet m_odb;
    private readonly IGitGraphProvider m_graphPrv;
    private readonly Func<IBitmap<Sha1Id>> m_setConstructor;

    public BasicReachabilityProvider(
      IVssRequestContext rc,
      IGitObjectSet repo,
      IGitGraphProvider graphPrv,
      ITwoWayReadOnlyList<Sha1Id> index,
      Func<IBitmap<Sha1Id>> setConstructor)
    {
      this.m_rc = rc;
      this.m_odb = repo;
      this.m_graphPrv = graphPrv;
      this.ObjectList = index;
      this.m_setConstructor = setConstructor;
    }

    public ITwoWayReadOnlyList<Sha1Id> ObjectList { get; }

    public IBitmap<Sha1Id> GetReachableIndexSet(
      IEnumerable<Sha1Id> reachableFromCommits,
      IEnumerable<Sha1Id> notReachableFromCommits,
      IReadOnlyBitmap<Sha1Id> notInSet,
      IObserver<int> statusObserver = null)
    {
      IGitCommitGraph gitCommitGraph = this.m_graphPrv.Get(reachableFromCommits);
      AncestralGraphAlgorithm<int, Sha1Id> ancestralGraphAlgorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
      IBitmap<Sha1Id> reachableIndexSet = this.m_setConstructor();
      IGitCommitGraph graph = gitCommitGraph;
      IEnumerable<Sha1Id> reachableFrom = reachableFromCommits;
      IEnumerable<Sha1Id> reachable = ancestralGraphAlgorithm.GetReachable((IDirectedGraph<int, Sha1Id>) graph, reachableFrom);
      Queue<Sha1Id> sha1IdQueue = new Queue<Sha1Id>();
      foreach (Sha1Id vertex in reachable)
      {
        int index1 = this.ObjectList.GetIndex<Sha1Id>(vertex);
        if ((notInSet == null || !notInSet.ContainsIndex(index1)) && reachableIndexSet.AddIndex(index1))
        {
          Sha1Id rootTreeId = gitCommitGraph.GetRootTreeId(gitCommitGraph.GetLabel(vertex));
          int index2 = this.ObjectList.GetIndex<Sha1Id>(rootTreeId);
          if ((notInSet == null || !notInSet.ContainsIndex(index1)) && reachableIndexSet.AddIndex(index2))
            sha1IdQueue.Enqueue(rootTreeId);
        }
      }
      while (sha1IdQueue.Count > 0)
      {
        foreach (TfsGitTreeEntry treeEntry in this.m_odb.LookupObject<TfsGitTree>(sha1IdQueue.Dequeue()).GetTreeEntries())
        {
          if (treeEntry.ObjectType == GitObjectType.Blob || treeEntry.ObjectType == GitObjectType.Tree)
          {
            int index = this.ObjectList.GetIndex<Sha1Id>(treeEntry.ObjectId);
            if ((notInSet == null || !notInSet.ContainsIndex(index)) && reachableIndexSet.AddIndex(index) && treeEntry.ObjectType == GitObjectType.Tree)
              sha1IdQueue.Enqueue(treeEntry.ObjectId);
          }
        }
      }
      return reachableIndexSet;
    }
  }
}
