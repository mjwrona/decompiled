// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.PathInfoOracle
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class PathInfoOracle : IPathInfoOracle
  {
    private readonly IGitObjectSet m_repo;
    private readonly IGitCommitGraph m_graph;
    private readonly NormalizedGitPath m_path;
    private readonly BloomKey m_key;
    private readonly int m_lastIndex;
    private Dictionary<int, Sha1Id?[]> m_objectIds;
    private Dictionary<int, GitObjectType> m_objectTypes;
    private Sha1Id?[] m_recentObjectIds;
    private GitObjectType m_recentObjectType;
    private readonly PathInfoStatistics m_statistics;

    public PathInfoOracle(IGitCommitGraph graph, IGitObjectSet repo, NormalizedGitPath path)
    {
      this.m_repo = repo;
      this.m_graph = graph;
      this.m_path = path;
      this.m_objectTypes = new Dictionary<int, GitObjectType>();
      this.m_objectIds = new Dictionary<int, Sha1Id?[]>();
      this.m_lastIndex = Math.Max(0, path.Parts.Count - 1);
      this.m_key = graph.EncodeKey(path.ToString());
      this.m_statistics = new PathInfoStatistics();
    }

    public PathInfoStatistics Statistics => this.m_statistics;

    public void ClearCachedTreesameInfo(int label)
    {
      this.m_objectIds.Remove(label);
      this.m_objectTypes.Remove(label);
    }

    public bool HasPath(int label)
    {
      Sha1Id?[] objectIds = (Sha1Id?[]) null;
      GitObjectType objectType1 = GitObjectType.Bad;
      if (!this.m_objectIds.TryGetValue(label, out objectIds))
      {
        ++this.m_statistics.NumWalkPathCalls;
        this.WalkPath(label, out objectIds, out objectType1);
        this.m_objectIds[label] = objectIds;
        this.m_objectTypes[label] = objectType1;
      }
      else
      {
        GitObjectType objectType2 = this.m_objectTypes[label];
      }
      return objectIds[this.m_lastIndex].HasValue;
    }

    public bool IsTreesameToFirstParent(int label)
    {
      ++this.m_statistics.NumTreesameFirstParentQueries;
      int? nullable = this.m_graph.OutNeighborsOfLabel(label).Select<int, int?>((Func<int, int?>) (i => new int?(i))).FirstOrDefault<int?>();
      if (!nullable.HasValue)
        return !this.HasPath(label);
      int label1 = nullable.Value;
      BloomFilterStatus filterStatus = this.m_graph.GetFilterStatus(label);
      if (filterStatus == BloomFilterStatus.Computed && !this.m_graph.GetReadOnlyFilter(label).ProbablyContains(this.m_key))
      {
        ++this.m_statistics.NumBloomFiltersTrueNegative;
        return true;
      }
      bool firstParent = this.IsPairTreesame(label, label1);
      switch (filterStatus)
      {
        case BloomFilterStatus.NotComputed:
          ++this.m_statistics.NumBloomFiltersNotComputed;
          break;
        case BloomFilterStatus.Computed:
          if (firstParent)
          {
            ++this.m_statistics.NumBloomFiltersFalsePositive;
            break;
          }
          ++this.m_statistics.NumBloomFiltersTruePositive;
          break;
        case BloomFilterStatus.TooLarge:
          ++this.m_statistics.NumBloomFiltersTooLarge;
          break;
      }
      return firstParent;
    }

    public bool IsPairTreesame(int label0, int label1)
    {
      ++this.m_statistics.NumTreesamePairQueries;
      Sha1Id?[] oldObjectIds;
      int num = this.m_objectIds.TryGetValue(label0, out oldObjectIds) ? 1 : 0;
      Sha1Id?[] newObjectIds;
      bool flag1 = this.m_objectIds.TryGetValue(label1, out newObjectIds);
      GitObjectType objectType;
      GitObjectType newObjectType;
      bool flag2;
      if (num != 0)
      {
        objectType = this.m_objectTypes[label0];
        if (flag1)
        {
          newObjectType = this.m_objectTypes[label1];
          Sha1Id? nullable1 = oldObjectIds[this.m_lastIndex];
          Sha1Id? nullable2 = newObjectIds[this.m_lastIndex];
          flag2 = nullable1.HasValue == nullable2.HasValue && (!nullable1.HasValue || nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault());
        }
        else
        {
          ++this.m_statistics.NumComparePathCalls;
          flag2 = TfsGitDiffHelper.ComparePaths(this.m_repo.LookupObject<TfsGitTree>(this.m_graph.GetRootTreeId(label1)), this.m_path, oldObjectIds, objectType, out newObjectIds, out newObjectType);
          this.m_objectIds[label1] = newObjectIds;
          this.m_objectTypes[label1] = newObjectType;
        }
      }
      else if (flag1)
      {
        newObjectType = this.m_objectTypes[label1];
        ++this.m_statistics.NumComparePathCalls;
        flag2 = TfsGitDiffHelper.ComparePaths(this.m_repo.LookupObject<TfsGitTree>(this.m_graph.GetRootTreeId(label0)), this.m_path, newObjectIds, newObjectType, out oldObjectIds, out objectType);
        this.m_objectIds[label0] = oldObjectIds;
        this.m_objectTypes[label0] = objectType;
      }
      else
      {
        ++this.m_statistics.NumWalkPathCalls;
        this.WalkPath(label0, out oldObjectIds, out objectType);
        this.m_objectIds[label0] = oldObjectIds;
        this.m_objectTypes[label0] = objectType;
        ++this.m_statistics.NumComparePathCalls;
        flag2 = TfsGitDiffHelper.ComparePaths(this.m_repo.LookupObject<TfsGitTree>(this.m_graph.GetRootTreeId(label1)), this.m_path, oldObjectIds, objectType, out newObjectIds, out newObjectType);
        this.m_objectIds[label1] = newObjectIds;
        this.m_objectTypes[label1] = newObjectType;
      }
      bool flag3 = flag2 & oldObjectIds[this.m_lastIndex].HasValue == newObjectIds[this.m_lastIndex].HasValue;
      if (flag3 && oldObjectIds[this.m_lastIndex].HasValue)
        flag3 = objectType == newObjectType && oldObjectIds[this.m_lastIndex].Value == newObjectIds[this.m_lastIndex].Value;
      this.m_recentObjectIds = newObjectIds;
      this.m_recentObjectType = newObjectType;
      return flag3;
    }

    public GitObjectType GetObjectType(int label)
    {
      GitObjectType objectType;
      if (!this.m_objectTypes.TryGetValue(label, out objectType))
      {
        ++this.m_statistics.NumWalkPathCalls;
        Sha1Id?[] objectIds;
        this.WalkPath(label, out objectIds, out objectType);
        this.m_objectTypes[label] = objectType;
        this.m_objectIds[label] = objectIds;
      }
      return objectType;
    }

    private void WalkPath(int label, out Sha1Id?[] objectIds, out GitObjectType objectType)
    {
      if (this.m_recentObjectIds == null)
      {
        TfsGitDiffHelper.WalkPath(this.m_repo.LookupObject<TfsGitTree>(this.m_graph.GetRootTreeId(label)), this.m_path, out objectIds, out objectType);
        this.m_recentObjectIds = objectIds;
        this.m_recentObjectType = objectType;
      }
      else
      {
        TfsGitDiffHelper.ComparePaths(this.m_repo.LookupObject<TfsGitTree>(this.m_graph.GetRootTreeId(label)), this.m_path, this.m_recentObjectIds, this.m_recentObjectType, out objectIds, out objectType);
        this.m_recentObjectIds = objectIds;
        this.m_recentObjectType = objectType;
      }
    }
  }
}
