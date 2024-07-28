// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.ReachabilityBitmapBuilder
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class ReachabilityBitmapBuilder
  {
    private readonly IVssRequestContext m_rc;
    private readonly IGitObjectSet m_repo;
    private readonly Sha1Id? m_stableOrderEpoch;
    private readonly IGitCommitGraph m_commitGraph;
    private readonly AncestralGraphAlgorithm<int, Sha1Id> m_graphAlgorithm;
    private readonly DeltaForestAlgorithm<int, Sha1Id> m_deltaAlgorithm;
    private readonly ObjectWalker m_walker;
    private readonly HashSet<Sha1Id> m_walkSet;
    private const string c_layer = "ReachabilityBitmapBuilder";
    private const int c_maxSelectionsPerLevel = 256;

    public ReachabilityBitmapBuilder(
      IVssRequestContext rc,
      IGitObjectSet repo,
      Sha1Id stableOrderEpoch,
      ITwoWayReadOnlyList<Sha1Id> objectList,
      IGitCommitGraph commitGraph)
    {
      this.m_rc = rc;
      this.m_repo = repo;
      this.m_stableOrderEpoch = new Sha1Id?(stableOrderEpoch);
      this.ObjectList = objectList;
      this.m_deltaAlgorithm = new DeltaForestAlgorithm<int, Sha1Id>();
      this.m_graphAlgorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
      this.m_walker = new ObjectWalker();
      this.m_walkSet = new HashSet<Sha1Id>();
      this.m_commitGraph = commitGraph;
      this.Stats = new ReachabilityBitmapBuilder.Statistics();
      this.Stats.NumCommitsInGraph = commitGraph.NumVertices;
    }

    public ITwoWayReadOnlyList<Sha1Id> ObjectList { get; }

    public ReachabilityBitmapCollection ComputeBitmaps(ReachabilityBitmapCollection baseCollection = null)
    {
      this.Stats.ComputationMillis = new Dictionary<string, long>();
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      Stopwatch stopwatch2 = Stopwatch.StartNew();
      List<int> labelsByDepth = this.GetLabelsByDepth();
      if (labelsByDepth.Count == 0)
        return new ReachabilityBitmapCollection(this.m_stableOrderEpoch.Value, this.ObjectList);
      this.Stats.MaxDepth = this.m_commitGraph.GetAncestryDepth(labelsByDepth[labelsByDepth.Count - 1]);
      this.Stats.ComputationMillis["GetLabelsByDepth"] = stopwatch2.ElapsedMilliseconds;
      stopwatch2.Restart();
      List<int> levelHeights = ReachabilityBitmapBuilder.GetLevelHeights(this.Stats.MaxDepth);
      List<HashSet<int>> candidatesInLayers = this.GetCandidatesInLayers(labelsByDepth, levelHeights);
      this.Stats.ComputationMillis["GetCandidatesInLayers"] = stopwatch2.ElapsedMilliseconds;
      stopwatch2.Restart();
      this.Stats.NumNonEmptyLevels = candidatesInLayers.Count;
      Dictionary<int, int> trunkiness = this.GetTrunkiness(candidatesInLayers);
      this.Stats.ComputationMillis["GetTrunkiness"] = stopwatch2.ElapsedMilliseconds;
      stopwatch2.Restart();
      List<HashSet<int>> selections = this.SelectCommitsForBitmaps(candidatesInLayers, trunkiness, baseCollection?.DeltaForest);
      this.Stats.ComputationMillis["SelectCommitsForBitmaps"] = stopwatch2.ElapsedMilliseconds;
      stopwatch2.Restart();
      Sha1IdDeltaForest forest = this.ConstructDeltaForest(selections);
      this.Stats.ComputationMillis["ConstructDeltaForest"] = stopwatch2.ElapsedMilliseconds;
      this.Stats.NumCommitsForBitmaps = (long) forest.NumVertices;
      stopwatch2.Restart();
      ReachabilityBitmapCollection bitmaps = this.BuildCollectionForForest(forest, baseCollection);
      this.Stats.ComputationMillis["BuildCollectionForForest"] = stopwatch2.ElapsedMilliseconds;
      stopwatch2.Restart();
      for (int label = 0; label < forest.NumVertices; ++label)
      {
        if (this.Stats.MaxDeltaChainLength < (long) forest.GetDeltaChainLength(label))
          this.Stats.MaxDeltaChainLength = (long) forest.GetDeltaChainLength(label);
      }
      this.Stats.NumObjectsInRepo = this.ObjectList.Count;
      this.Stats.TotalMillis = stopwatch1.ElapsedMilliseconds;
      this.m_rc.TraceAlways(1013729, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (ReachabilityBitmapBuilder), JsonConvert.SerializeObject((object) this.Stats));
      return bitmaps;
    }

    internal List<int> GetLabelsByDepth()
    {
      List<int> list = this.m_commitGraph.GetLabels(this.m_commitGraph.GetVertices()).ToList<int>();
      list.Sort((Comparison<int>) ((l1, l2) => this.m_commitGraph.GetAncestryDepth(l1) - this.m_commitGraph.GetAncestryDepth(l2)));
      return list;
    }

    internal static int GetMinimumLevelHeight(int numVertices) => numVertices <= 4 ? 2 : Math.Min(64, 1 << Math.Max(1, (int) Math.Log((double) numVertices, 4.0) - 3));

    internal static int GetMaximumLevelHeight(int numVertices) => Math.Min(2048, 1 << Math.Max(7, (int) Math.Log((double) numVertices, 4.0) + 2));

    internal static List<int> GetLevelHeights(int maxDepth)
    {
      if (maxDepth == 0)
        return new List<int>() { 0 };
      int num1 = maxDepth / 2;
      int num2 = ReachabilityBitmapBuilder.GetMaximumLevelHeight(maxDepth);
      int minimumLevelHeight = ReachabilityBitmapBuilder.GetMinimumLevelHeight(maxDepth);
      List<int> levelHeights = new List<int>();
      int num3 = 0;
      while (num3 <= maxDepth)
      {
        levelHeights.Add(num3);
        num3 += num2;
        if (num2 > minimumLevelHeight && num3 > num1)
        {
          int num4 = num3 - num2;
          num2 = Math.Max(num2 / 2, 1);
          num3 = num4 + num2;
          num1 += (maxDepth - num1) / 2;
        }
      }
      return levelHeights;
    }

    internal List<HashSet<int>> GetCandidatesInLayers(
      List<int> labelsByDepth,
      List<int> levelHeights)
    {
      List<HashSet<int>> candidatesInLayers = new List<HashSet<int>>();
      int index1 = 0;
      HashSet<int> intSet = new HashSet<int>();
      int num1 = levelHeights[0];
      for (int index2 = 0; index2 < labelsByDepth.Count; ++index2)
      {
        int label = labelsByDepth[index2];
        if (this.m_commitGraph.GetAncestryDepth(label) > num1)
        {
          candidatesInLayers.Add(intSet);
          intSet = new HashSet<int>();
          ++index1;
          num1 = index1 >= levelHeights.Count ? this.m_commitGraph.NumVertices : levelHeights[index1];
        }
        intSet.Add(label);
        foreach (int num2 in this.m_commitGraph.OutNeighborsOfLabel(label))
          intSet.Remove(num2);
      }
      candidatesInLayers.Add(intSet);
      return candidatesInLayers;
    }

    internal Dictionary<int, int> GetTrunkiness(List<HashSet<int>> candidates)
    {
      Dictionary<int, int> dictionary = new Dictionary<int, int>(this.m_commitGraph.NumVertices);
      int minDepth = 0;
      SubgraphWrapper<int, Sha1Id> subgraphWrapper = new SubgraphWrapper<int, Sha1Id>((IDirectedGraph<int, Sha1Id>) this.m_commitGraph, (Predicate<int>) (label => this.m_commitGraph.GetAncestryDepth(label) >= minDepth));
      int capacity = 0;
      for (int index = 0; index < candidates.Count; ++index)
      {
        capacity += candidates[index].Count;
        foreach (int key in candidates[index])
          dictionary[key] = 1;
      }
      foreach (int reachableLabel in this.m_graphAlgorithm.GetReachableLabels((IDirectedGraph<int, Sha1Id>) this.m_commitGraph, candidates.SelectMany<HashSet<int>, int>((Func<HashSet<int>, IEnumerable<int>>) (set => (IEnumerable<int>) set))))
      {
        int num1;
        if (dictionary.TryGetValue(reachableLabel, out num1))
        {
          using (IEnumerator<int> enumerator = this.m_commitGraph.OutNeighborsOfLabel(reachableLabel).GetEnumerator())
          {
            if (enumerator.MoveNext())
            {
              int current = enumerator.Current;
              int num2;
              if (!dictionary.TryGetValue(current, out num2))
                num2 = 0;
              dictionary[current] = num2 + num1;
            }
          }
        }
      }
      Dictionary<int, int> trunkiness = new Dictionary<int, int>(capacity);
      for (int index = 0; index < candidates.Count; ++index)
      {
        foreach (int key in candidates[index])
          trunkiness[key] = dictionary[key];
      }
      return trunkiness;
    }

    internal List<HashSet<int>> SelectCommitsForBitmaps(
      List<HashSet<int>> candidates,
      Dictionary<int, int> trunkiness,
      IDeltaForest<int, Sha1Id> existingForest = null)
    {
      if (candidates.Count == 0)
        return new List<HashSet<int>>();
      List<HashSet<int>> intSetList = new List<HashSet<int>>();
      for (int index = 0; index < candidates.Count; ++index)
        intSetList.Add(new HashSet<int>());
      this.Stats.NumCommitsInMaxLevel = candidates.Max<HashSet<int>>((Func<HashSet<int>, int>) (list => list.Count));
      int index1 = candidates.Count - 1;
      HashSet<int> candidate1 = candidates[index1];
      this.Stats.NumCommitsInTopLevel = candidate1.Count;
      int num1 = Math.Min(256, candidate1.Count);
      int num2 = candidate1.Count / num1;
      int num3 = 0;
      foreach (int num4 in candidate1)
      {
        if (num3 % num2 == 0)
          intSetList[index1].Add(num4);
        ++num3;
      }
      int val1_1 = 256;
      int val1_2 = (int) Math.Max(2.0, Math.Log((double) this.m_commitGraph.NumVertices, 2.0) - 8.0);
      SetCoverFactory<Sha1Id> setCoverFactory = new SetCoverFactory<Sha1Id>((IDirectedGraph<int, Sha1Id>) this.m_commitGraph);
      List<int> intList = intSetList[index1].ToList<int>();
      for (int index2 = candidates.Count - 2; index2 > 0; --index2)
      {
        val1_1 = Math.Max(val1_2, (int) ((double) val1_1 * 0.75));
        HashSet<int> candidate2 = candidates[index2];
        if (candidate2.Count > 0)
        {
          List<int> list = candidate2.ToList<int>();
          int numToSelect = Math.Min(val1_1, list.Count);
          Math.Max(1, list.Count / numToSelect);
          if (numToSelect <= 2)
            numToSelect = 2;
          Dictionary<int, RoaringBitmap<int>> reachabilityBetweenLabels = setCoverFactory.GetReachabilityBetweenLabels(intList, list);
          List<int> uncovered;
          intSetList[index2] = this.MakeLevelSelections(numToSelect, list, intList, reachabilityBetweenLabels, trunkiness, out uncovered);
          intList = uncovered;
        }
      }
      intSetList[0] = candidates[0];
      return intSetList;
    }

    internal HashSet<int> MakeLevelSelections(
      int numToSelect,
      List<int> candidates,
      List<int> toCover,
      Dictionary<int, RoaringBitmap<int>> coverage,
      Dictionary<int, int> trunkiness,
      out List<int> uncovered)
    {
      HashSet<int> source = new HashSet<int>(SetCoverAlgorithm<int>.Instance.GetCover(coverage).Take<int>(numToSelect));
      candidates.Sort((Comparison<int>) ((i1, i2) => trunkiness[i2].CompareTo(trunkiness[i1])));
      int index1 = 0;
      while (index1 < candidates.Count && trunkiness[candidates[index1]] > 1)
        ++index1;
      if (source.Count < numToSelect && index1 > 0 && numToSelect > source.Count)
      {
        int index2 = 0;
        while (index2 < index1 && (!source.Add(candidates[index2]) || source.Count < numToSelect))
          ++index2;
      }
      if (index1 < candidates.Count)
      {
        int num1 = Math.Max(1, (candidates.Count - index1) / numToSelect);
        int num2 = 0;
        int index3 = index1;
        while (index3 < candidates.Count && (!source.Add(candidates[index3]) || ++num2 < numToSelect))
          index3 += num1;
      }
      RoaringBitmap<int> roaringBitmap = new RoaringBitmapCombiner<int>((ITwoWayReadOnlyList<int>) new ListWrapper(toCover)).Combine(source.Select<int, RoaringBitmap<int>>((Func<int, RoaringBitmap<int>>) (label => coverage[label])));
      uncovered = new List<int>();
      for (int index4 = 0; index4 < toCover.Count; ++index4)
      {
        if (!roaringBitmap.ContainsIndex(index4))
          uncovered.Add(toCover[index4]);
      }
      for (int index5 = 0; index5 < candidates.Count; ++index5)
      {
        if (!source.Contains(candidates[index5]))
          uncovered.Add(candidates[index5]);
      }
      return source;
    }

    internal Sha1IdDeltaForest ConstructDeltaForest(List<HashSet<int>> selections)
    {
      Sha1IdDeltaForest sha1IdDeltaForest = new Sha1IdDeltaForest();
      for (int index1 = 0; index1 < selections.Count; ++index1)
      {
        foreach (int num in selections[index1])
        {
          int label = -1;
          int index2 = index1 - 1;
          while (index2 >= 0)
          {
            foreach (int target in selections[index2])
            {
              if (this.m_graphAlgorithm.CanReachLabels((IDirectedGraph<int, Sha1Id>) this.m_commitGraph, num, target))
              {
                label = target;
                break;
              }
            }
            if (label >= 0)
            {
              sha1IdDeltaForest.AddDeltaVertex(this.m_commitGraph.GetVertex(num), this.m_commitGraph.GetVertex(label));
              ++this.Stats.NumDeltaCommits;
              break;
            }
            --index2;
            ++this.Stats.NumCarryOvers;
          }
          if (label < 0)
          {
            sha1IdDeltaForest.AddBaseVertex(this.m_commitGraph.GetVertex(num));
            ++this.Stats.NumBaseCommits;
          }
        }
      }
      return sha1IdDeltaForest;
    }

    internal ReachabilityBitmapCollection BuildCollectionForForest(
      Sha1IdDeltaForest forest,
      ReachabilityBitmapCollection baseCollection = null)
    {
      ReachabilityBitmapCollection bitmapCollection1 = (ReachabilityBitmapCollection) null;
      try
      {
        bitmapCollection1 = new ReachabilityBitmapCollection(this.m_stableOrderEpoch.Value, this.ObjectList);
        for (int label1 = 0; label1 < forest.NumVertices; ++label1)
        {
          Sha1Id vertex = forest.GetVertex(label1);
          RoaringBitmap<Sha1Id> notInSet = (RoaringBitmap<Sha1Id>) null;
          Sha1Id? parentId = new Sha1Id?();
          int parent;
          if (forest.TryGetParent(label1, out parent))
          {
            notInSet = bitmapCollection1.Combine((IEnumerable<int>) this.m_deltaAlgorithm.CompareDeltaChains(bitmapCollection1.DeltaForest, (IEnumerable<int>) new int[1]
            {
              parent
            }, (IEnumerable<int>) null).ReachableLabels, (RoaringBitmap<Sha1Id>) null);
            parentId = new Sha1Id?(forest.GetVertex(parent));
          }
          RoaringBitmap<Sha1Id> bitmap;
          if (baseCollection != null && baseCollection.DeltaForest.HasVertex(vertex))
          {
            int label2 = baseCollection.DeltaForest.GetLabel(vertex);
            bitmap = baseCollection.Combine((IEnumerable<int>) this.m_deltaAlgorithm.CompareDeltaChains(baseCollection.DeltaForest, (IEnumerable<int>) new int[1]
            {
              label2
            }, (IEnumerable<int>) null).ReachableLabels, notInSet);
            ++this.Stats.NumReuseBitmaps;
          }
          else
            bitmap = this.ConstructBitmap(vertex, notInSet, baseCollection);
          bitmapCollection1.AddBitmap(vertex, bitmap, parentId);
        }
        ReachabilityBitmapCollection bitmapCollection2 = bitmapCollection1;
        bitmapCollection1 = (ReachabilityBitmapCollection) null;
        return bitmapCollection2;
      }
      finally
      {
        bitmapCollection1?.Dispose();
      }
    }

    internal RoaringBitmap<Sha1Id> ConstructBitmap(
      Sha1Id fromCommitId,
      RoaringBitmap<Sha1Id> notInSet,
      ReachabilityBitmapCollection baseCollection = null)
    {
      ++this.Stats.NumBitmapsComputed;
      RoaringBitmap<Sha1Id> bitmap = new RoaringBitmap<Sha1Id>(this.ObjectList);
      IDirectedGraph<int, Sha1Id> graph = (IDirectedGraph<int, Sha1Id>) this.m_commitGraph;
      if (notInSet != null)
        graph = (IDirectedGraph<int, Sha1Id>) new SubgraphWrapper<int, Sha1Id>((IDirectedGraph<int, Sha1Id>) this.m_commitGraph, (Predicate<int>) (label => !notInSet.Contains<Sha1Id>(this.m_commitGraph.GetVertex(label))));
      List<Sha1Id> source = new List<Sha1Id>();
      this.m_walkSet.Clear();
      foreach (int reachableLabel in this.m_graphAlgorithm.GetReachableLabels(graph, (IEnumerable<int>) new int[1]
      {
        this.m_commitGraph.GetLabel(fromCommitId)
      }))
      {
        Sha1Id vertex = this.m_commitGraph.GetVertex(reachableLabel);
        if (this.m_walkSet.Add(vertex) && bitmap.Add(vertex))
        {
          if (baseCollection != null && baseCollection.DeltaForest.HasVertex(vertex))
          {
            int label = baseCollection.DeltaForest.GetLabel(vertex);
            RoaringBitmap<Sha1Id> withBitmap = baseCollection.Combine((IEnumerable<int>) this.m_deltaAlgorithm.CompareDeltaChains(baseCollection.DeltaForest, (IEnumerable<int>) new int[1]
            {
              label
            }, (IEnumerable<int>) null).ReachableLabels, notInSet);
            ++this.Stats.NumCombineExistingBitmaps;
            bitmap = RoaringBitmapCombiner<Sha1Id>.Union(bitmap, withBitmap);
          }
          else
            source.Add(this.m_commitGraph.GetRootTreeId(reachableLabel));
        }
      }
      this.m_walker.Walk(source.Select<Sha1Id, TfsGitObject>((Func<Sha1Id, TfsGitObject>) (treeId => this.m_repo.LookupObject(treeId))), (Func<Sha1Id, GitObjectType, bool>) ((id, objectType) =>
      {
        if (!this.m_walkSet.Add(id))
          return false;
        ++this.Stats.NumObjectGraphEdgesWalked;
        int index = this.ObjectList.GetIndex<Sha1Id>(id);
        RoaringBitmap<Sha1Id> roaringBitmap = notInSet;
        return (roaringBitmap != null ? (roaringBitmap.ContainsIndex(index) ? 1 : 0) : 0) == 0 && bitmap.AddIndex(index);
      }));
      bitmap.MakeReadOnly();
      return bitmap;
    }

    internal ReachabilityBitmapBuilder.Statistics Stats { get; private set; }

    internal class Statistics
    {
      public long TotalMillis { get; set; }

      public Dictionary<string, long> ComputationMillis { get; set; }

      public long NumCommitsForBitmaps { get; set; }

      public long NumBitmapsComputed { get; set; }

      public long NumObjectGraphEdgesWalked { get; set; }

      public int MaxDepth { get; set; }

      public long MaxDeltaChainLength { get; set; }

      public long NumCarryOvers { get; set; }

      public long NumDeltaCommits { get; set; }

      public long NumBaseCommits { get; set; }

      public int NumNonEmptyLevels { get; set; }

      public int NumReuseBitmaps { get; set; }

      public int NumCombineExistingBitmaps { get; set; }

      public int NumObjectsInRepo { get; set; }

      public int NumCommitsInGraph { get; set; }

      public int NumCommitsInTopLevel { get; set; }

      public int NumCommitsInMaxLevel { get; set; }
    }
  }
}
