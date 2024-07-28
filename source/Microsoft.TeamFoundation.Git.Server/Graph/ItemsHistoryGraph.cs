// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.ItemsHistoryGraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class ItemsHistoryGraph : CachedGraphWrapper
  {
    private readonly List<TfsGitTreeEntry> m_itemFullList;
    private SortedSet<int> m_itemsStillActive;
    private Dictionary<int, List<int>> m_changedItemIndices;
    private List<int> m_neighborList;
    private HashSet<int> m_neighborSet;
    private List<int> m_itemsToRemove;
    private readonly BloomKey m_pathKey;
    private readonly BloomKey[] m_itemKeys;
    private IGitObjectSet m_repository;
    private NormalizedGitPath m_path;
    private int m_numAllowedNullResponses;
    private int m_numBufferCommits;
    private int m_maxBufferCommits;
    private Dictionary<int, Sha1Id?[]> m_objectIds;
    private const int c_itemIsParentFolder = -1;

    public ItemsHistoryGraph(
      IGitCommitGraph graph,
      IGitObjectSet repository,
      NormalizedGitPath parentPath,
      TfsGitTree parentTree,
      double requiredProportion = 0.9,
      int maxBufferCommits = 100)
      : base(graph)
    {
      this.m_repository = repository;
      this.m_path = parentPath;
      this.m_itemFullList = parentTree.GetTreeEntries().ToList<TfsGitTreeEntry>();
      this.m_itemsStillActive = new SortedSet<int>(Enumerable.Range(0, this.m_itemFullList.Count));
      if (parentPath.Parts.Count > 0)
        this.m_itemsStillActive.Add(-1);
      this.m_numAllowedNullResponses = (int) ((1.0 - requiredProportion) * (double) this.m_itemFullList.Count);
      this.m_maxBufferCommits = maxBufferCommits;
      this.m_changedItemIndices = new Dictionary<int, List<int>>();
      this.m_objectIds = new Dictionary<int, Sha1Id?[]>();
      this.m_neighborList = new List<int>();
      this.m_neighborSet = new HashSet<int>();
      this.m_itemsToRemove = new List<int>();
      this.m_pathKey = graph.EncodeKey(parentPath.ToString());
      this.m_itemKeys = new BloomKey[this.m_itemFullList.Count];
      for (int index = 0; index < this.m_itemFullList.Count; ++index)
      {
        TfsGitTreeEntry itemFull = this.m_itemFullList[index];
        this.m_itemKeys[index] = graph.EncodeKey(new NormalizedGitPath(parentPath?.ToString() + "/" + itemFull.Name).ToString());
      }
    }

    public override void ClearLabel(int label)
    {
      base.ClearLabel(label);
      foreach (int num in this.m_changedItemIndices[label])
        this.m_itemsStillActive.Remove(num);
      this.m_changedItemIndices.Remove(label);
      this.m_objectIds.Remove(label);
    }

    public List<LatestChange> GetItemsChangedAtLabel(int label)
    {
      if (!this.IsLabelAccepted(label))
        return new List<LatestChange>();
      List<int> changedItemIndex = this.m_changedItemIndices[label];
      Sha1Id commitId = this.GetVertex(label);
      Func<int, LatestChange> selector = (Func<int, LatestChange>) (item => item != -1 ? new LatestChange(this.m_itemFullList[item].Name, commitId, this.m_itemFullList[item].ObjectType == GitObjectType.Tree) : new LatestChange(string.Empty, commitId, true));
      return changedItemIndex.Select<int, LatestChange>(selector).ToList<LatestChange>();
    }

    protected override int[] ComputeDataForLabel(int label)
    {
      if (this.m_itemsStillActive.Count < this.m_numAllowedNullResponses)
      {
        if (this.m_numBufferCommits >= this.m_maxBufferCommits)
          return Array.Empty<int>();
        ++this.m_numBufferCommits;
      }
      List<int> first;
      List<int> intList1 = !this.m_changedItemIndices.TryGetValue(label, out first) ? new List<int>((IEnumerable<int>) this.m_itemsStillActive) : new List<int>(first.Intersect<int>((IEnumerable<int>) this.m_itemsStillActive));
      if (this.m_commitGraph.GetFilterStatus(label) == BloomFilterStatus.Computed)
      {
        bool flag = true;
        IReadOnlyBloomFilter readOnlyFilter = this.m_commitGraph.GetReadOnlyFilter(label);
        if (intList1.Count > 0)
        {
          foreach (int index in intList1)
          {
            flag = index >= 0 ? readOnlyFilter.ProbablyContains(this.m_itemKeys[index]) : this.m_path.Parts.Count == 0 || readOnlyFilter.ProbablyContains(this.m_pathKey);
            if (flag)
              break;
          }
        }
        if (!flag)
        {
          this.m_changedItemIndices[label] = new List<int>();
          int[] array = this.m_graph.OutNeighborsOfLabel(label).Take<int>(1).ToArray<int>();
          if (array.Length != 0)
          {
            int key = array[0];
            List<int> intList2 = (List<int>) null;
            if (!this.m_changedItemIndices.TryGetValue(key, out intList2))
            {
              intList2 = new List<int>();
              this.m_changedItemIndices[key] = intList2;
            }
            foreach (int num in intList1)
              intList2.Add(num);
          }
          return array;
        }
      }
      Sha1Id?[] objectIds = (Sha1Id?[]) null;
      Sha1Id? nullable1 = new Sha1Id?();
      if (this.m_path.Parts.Count == 0)
      {
        objectIds = Array.Empty<Sha1Id?>();
        nullable1 = new Sha1Id?(this.m_commitGraph.GetRootTreeId(label));
      }
      else if (this.m_objectIds.TryGetValue(label, out objectIds))
      {
        nullable1 = objectIds[objectIds.Length - 1];
      }
      else
      {
        GitObjectType objectType;
        TfsGitDiffHelper.WalkPath(this.m_repository.LookupObject<TfsGitTree>(this.m_commitGraph.GetRootTreeId(label)), this.m_path, out objectIds, out objectType);
        if (objectType != GitObjectType.Tree)
          objectIds[objectIds.Length - 1] = new Sha1Id?();
        nullable1 = objectIds[objectIds.Length - 1];
        this.m_objectIds[label] = objectIds;
      }
      if (!nullable1.HasValue)
      {
        this.m_changedItemIndices[label] = new List<int>();
        return Array.Empty<int>();
      }
      IEnumerable<int> ints = this.m_commitGraph.OutNeighborsOfLabel(label);
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      SortedSet<int> sortedSet = new SortedSet<int>((IEnumerable<int>) intList1);
      foreach (int num1 in ints)
      {
        if (!this.m_changedItemIndices.ContainsKey(num1))
          this.m_changedItemIndices[num1] = new List<int>();
        TfsGitTree tfsGitTree = (TfsGitTree) null;
        Sha1Id? nullable2;
        Sha1Id? nullable3;
        Sha1Id? nullable4;
        if (this.m_path.Parts.Count == 0)
        {
          nullable2 = new Sha1Id?(this.m_commitGraph.GetRootTreeId(num1));
          tfsGitTree = this.m_repository.LookupObject<TfsGitTree>(nullable2.Value);
        }
        else
        {
          Sha1Id?[] newObjectIds;
          GitObjectType newObjectType;
          if (this.m_objectIds.TryGetValue(num1, out newObjectIds))
          {
            nullable2 = newObjectIds[newObjectIds.Length - 1];
            newObjectType = GitObjectType.Tree;
          }
          else
          {
            TfsGitDiffHelper.ComparePaths(this.m_repository.LookupObject<TfsGitTree>(this.m_commitGraph.GetRootTreeId(num1)), this.m_path, objectIds, GitObjectType.Tree, out newObjectIds, out newObjectType);
            this.m_objectIds[num1] = newObjectIds;
          }
          Sha1Id? nullable5;
          if (newObjectType != GitObjectType.Tree)
          {
            nullable3 = new Sha1Id?();
            nullable5 = nullable3;
          }
          else
            nullable5 = newObjectIds[newObjectIds.Length - 1];
          nullable2 = nullable5;
          if (nullable2.HasValue)
          {
            nullable3 = nullable2;
            nullable4 = nullable1;
            if ((nullable3.HasValue == nullable4.HasValue ? (nullable3.HasValue ? (nullable3.GetValueOrDefault() != nullable4.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
              tfsGitTree = this.m_repository.LookupObject<TfsGitTree>(nullable2.Value);
          }
        }
        nullable4 = nullable1;
        nullable3 = nullable2;
        if ((nullable4.HasValue == nullable3.HasValue ? (nullable4.HasValue ? (nullable4.GetValueOrDefault() == nullable3.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
        {
          foreach (int key in intList1)
            dictionary[key] = num1;
          intList1.Clear();
          break;
        }
        if (tfsGitTree != null)
        {
          this.m_itemsToRemove.Clear();
          IEnumerable<TfsGitTreeEntry> treeEntries = tfsGitTree.GetTreeEntries();
          foreach (int treesameIndex in this.m_itemFullList.GetTreesameIndices((IEnumerable<int>) intList1, treeEntries))
          {
            dictionary[treesameIndex] = num1;
            this.m_itemsToRemove.Add(treesameIndex);
          }
          foreach (int num2 in this.m_itemsToRemove)
            intList1.Remove(num2);
        }
      }
      this.m_neighborSet.Clear();
      List<int> intList3 = new List<int>();
      foreach (int key1 in this.m_itemsStillActive)
      {
        if (sortedSet.Contains(key1))
        {
          int key2;
          if (dictionary.TryGetValue(key1, out key2))
          {
            this.m_changedItemIndices[key2].Add(key1);
            this.m_neighborSet.Add(key2);
          }
          else
          {
            intList3.Add(key1);
            this.SetLabelAccepted(label);
          }
        }
      }
      this.m_changedItemIndices[label] = intList3;
      foreach (int num in intList3)
        this.m_itemsStillActive.Remove(num);
      this.m_neighborList.Clear();
      foreach (int num in ints)
      {
        if (this.m_neighborSet.Contains(num))
          this.m_neighborList.Add(num);
      }
      return this.m_neighborList.ToArray();
    }
  }
}
