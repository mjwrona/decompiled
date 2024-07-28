// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitCommitBatchHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitCommitBatchHelper
  {
    public static IList<ReachableSetAndBoundary<Sha1Id>> GetCommitsBatch(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<Tuple<Sha1Id, Sha1Id>> commitIdTuples)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<IEnumerable<Tuple<Sha1Id, Sha1Id>>>(commitIdTuples, nameof (commitIdTuples));
      List<TfsGitCommitRange> commitRanges = new List<TfsGitCommitRange>();
      Queue<int> intQueue = new Queue<int>();
      int num1 = 0;
      Sha1Id? nullable1 = new Sha1Id?();
      foreach (Tuple<Sha1Id, Sha1Id> commitIdTuple in commitIdTuples)
      {
        Sha1Id sha1Id1 = commitIdTuple.Item1;
        Sha1Id sha1Id2 = commitIdTuple.Item2;
        if (sha1Id1 == Sha1Id.Empty || sha1Id2 == Sha1Id.Empty)
        {
          intQueue.Enqueue(num1);
        }
        else
        {
          if (nullable1.HasValue)
          {
            Sha1Id? nullable2 = nullable1;
            Sha1Id empty = Sha1Id.Empty;
            if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
            {
              commitRanges.Add(new TfsGitCommitRange((IEnumerable<Sha1Id>) new Sha1Id[1]
              {
                sha1Id1
              }, (IEnumerable<Sha1Id>) new Sha1Id[2]
              {
                sha1Id2,
                nullable1.Value
              }));
              goto label_8;
            }
          }
          commitRanges.Add(new TfsGitCommitRange((IEnumerable<Sha1Id>) new Sha1Id[1]
          {
            sha1Id1
          }, (IEnumerable<Sha1Id>) new Sha1Id[1]{ sha1Id2 }));
        }
label_8:
        nullable1 = new Sha1Id?(sha1Id1);
        ++num1;
      }
      int capacity = num1;
      List<ReachableSetAndBoundary<Sha1Id>> commitHistoryBatch = repository.GetCommitHistoryBatch(requestContext, (IList<TfsGitCommitRange>) commitRanges);
      List<ReachableSetAndBoundary<Sha1Id>> commitsBatch = new List<ReachableSetAndBoundary<Sha1Id>>(capacity);
      int num2 = intQueue.Count != 0 ? intQueue.Dequeue() : -1;
      int index1 = 0;
      for (int index2 = 0; index2 < capacity; ++index2)
      {
        if (index2 == num2)
        {
          commitsBatch.Add(new ReachableSetAndBoundary<Sha1Id>((ISet<Sha1Id>) new HashSet<Sha1Id>(), (ISet<Sha1Id>) new HashSet<Sha1Id>()));
          num2 = intQueue.Count != 0 ? intQueue.Dequeue() : -1;
        }
        else
        {
          commitsBatch.Add(commitHistoryBatch[index1]);
          ++index1;
        }
      }
      return (IList<ReachableSetAndBoundary<Sha1Id>>) commitsBatch;
    }
  }
}
