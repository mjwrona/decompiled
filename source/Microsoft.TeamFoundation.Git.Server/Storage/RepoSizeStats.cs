// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Storage.RepoSizeStats
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server.Storage
{
  internal class RepoSizeStats
  {
    private readonly Dictionary<GitPackObjectType, int> m_reachableObjectCounts;
    private readonly Dictionary<GitPackObjectType, long> m_reachableObjectSizes;
    private static readonly IReadOnlyList<GitPackObjectType> s_countingTypes = (IReadOnlyList<GitPackObjectType>) new GitPackObjectType[4]
    {
      GitPackObjectType.Blob,
      GitPackObjectType.Tree,
      GitPackObjectType.Commit,
      GitPackObjectType.Tag
    };

    public RepoSizeStats(
      int numObjectsInIndex,
      int numPackfiles,
      int numRefs,
      int numPackfileCapSize)
    {
      this.NumObjectsInIndex = numObjectsInIndex;
      this.NumPackfilesInIndex = numPackfiles;
      this.NumRefs = numRefs;
      this.PackfileCapSize = numPackfileCapSize;
      this.m_reachableObjectCounts = new Dictionary<GitPackObjectType, int>();
      this.m_reachableObjectSizes = new Dictionary<GitPackObjectType, long>();
      foreach (GitPackObjectType countingType in (IEnumerable<GitPackObjectType>) RepoSizeStats.s_countingTypes)
      {
        this.m_reachableObjectCounts.Add(countingType, 0);
        this.m_reachableObjectSizes.Add(countingType, 0L);
      }
    }

    public int NumObjectsInIndex { get; }

    public int NumPackfilesInIndex { get; }

    public int NumReachableObjects { get; private set; }

    public int NumRefs { get; }

    public int PackfileCapSize { get; }

    public IReadOnlyDictionary<GitPackObjectType, int> ReachableObjectCounts => (IReadOnlyDictionary<GitPackObjectType, int>) this.m_reachableObjectCounts;

    public IReadOnlyDictionary<GitPackObjectType, long> ReachableObjectSizes => (IReadOnlyDictionary<GitPackObjectType, long>) this.m_reachableObjectSizes;

    public long OverallReachableSize { get; private set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddObject(GitPackObjectType type, long size)
    {
      this.m_reachableObjectCounts[type]++;
      this.m_reachableObjectSizes[type] += size;
      ++this.NumReachableObjects;
      this.OverallReachableSize += size;
    }
  }
}
