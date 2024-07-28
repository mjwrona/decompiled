// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.SimplifyMergesGraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class SimplifyMergesGraph : CachedGraphWrapper, IChangeTypeOracle
  {
    private IPathInfoOracle m_pathOracle;
    private readonly IntGraph m_simplifiedGraph;
    private readonly AncestralGraphAlgorithm<int, Sha1Id> m_algorithm;
    private readonly Dictionary<int, ChangeAndObjectType> m_changeTypes;
    private readonly Dictionary<int, int> m_simplification;
    private const int c_labelRemoved = -1;
    private const int c_parentRemoved = -2;
    private HashSet<int> m_firstParentPath;
    private List<int> m_parents;
    private List<int> m_parentsSwap;
    private readonly SimplifyMergesStatistics m_statistics;

    public SimplifyMergesGraph(
      IGitCommitGraph graph,
      IGitObjectSet repo,
      NormalizedGitPath path,
      Sha1Id startCommit)
      : this(graph, (IPathInfoOracle) new PathInfoOracle(graph, repo, path), startCommit)
    {
    }

    public SimplifyMergesGraph(
      IGitCommitGraph graph,
      IPathInfoOracle pathOracle,
      Sha1Id startCommit)
      : base(graph)
    {
      this.m_pathOracle = pathOracle;
      this.m_algorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
      this.m_changeTypes = new Dictionary<int, ChangeAndObjectType>();
      this.m_firstParentPath = new HashSet<int>();
      this.m_statistics = new SimplifyMergesStatistics();
      this.m_simplification = new Dictionary<int, int>();
      this.m_simplifiedGraph = new IntGraph();
      int label = graph.GetLabel(startCommit);
      Stopwatch stopwatch = Stopwatch.StartNew();
      Stack<int> labels = this.WalkGraph(label);
      this.m_statistics.WalkGraphMillis = stopwatch.ElapsedMilliseconds;
      stopwatch.Restart();
      this.SimplifyAllLabels(labels);
      this.m_statistics.SimplifyAllLabelsMillis = stopwatch.ElapsedMilliseconds;
      this.m_pathOracle = (IPathInfoOracle) null;
      this.m_firstParentPath = (HashSet<int>) null;
    }

    public override void ClearLabel(int label) => base.ClearLabel(label);

    public SimplifyMergesStatistics Statistics => this.m_statistics;

    protected override int[] ComputeDataForLabel(int label)
    {
      int simplification = this.GetSimplification(label);
      if (simplification != label)
        return new int[1]{ simplification };
      this.SetLabelAccepted(label);
      return this.m_simplifiedGraph.OutNeighbors(label).ToArray<int>();
    }

    public bool TryGetChangeType(Sha1Id commitId, out ChangeAndObjectType changeType)
    {
      int label = this.GetLabel(commitId);
      if (this.GetSimplification(label) == label)
        return this.m_changeTypes.TryGetValue(label, out changeType);
      changeType = new ChangeAndObjectType();
      return false;
    }

    private Stack<int> WalkGraph(int label)
    {
      Stack<int> intStack = new Stack<int>();
      PriorityQueue<int, long> priorityQueue = new PriorityQueue<int, long>((IEqualityComparer<int>) EqualityComparer<int>.Default, (IComparer<long>) new StackComparer());
      this.m_firstParentPath.Add(label);
      priorityQueue.EnqueueOrUpdate(label, (long) this.m_graph.GetAncestryDepth(label));
      int label1;
      while (priorityQueue.TryDequeueBeforeThreshold(long.MinValue, out label1))
      {
        intStack.Push(label1);
        bool flag = this.m_firstParentPath.Contains(label1);
        foreach (int label2 in this.m_graph.OutNeighborsOfLabel(label1))
        {
          priorityQueue.EnqueueOrUpdate(label2, (long) this.m_graph.GetAncestryDepth(label2));
          if (flag)
          {
            this.m_firstParentPath.Add(label2);
            flag = false;
          }
        }
      }
      return intStack;
    }

    private void SimplifyAllLabels(Stack<int> labels)
    {
      this.m_parents = new List<int>(2);
      this.m_parentsSwap = new List<int>(2);
      while (labels.Count > 0)
      {
        int num = labels.Pop();
        bool hadParentsBeforeSimplification;
        bool firstParentStillInSet;
        this.ComputeSimplifiedParents(num, out hadParentsBeforeSimplification, out firstParentStillInSet);
        this.TryCollapseMerge(num, ref firstParentStillInSet);
        this.SimplifyIfOneParent(num, firstParentStillInSet);
        this.SimplifyIfZeroParents(num, hadParentsBeforeSimplification);
        if (this.GetSimplification(num) == num)
          this.m_simplifiedGraph.AddVertex(num, this.m_parents);
      }
      this.m_statistics.NumSimplifiedVertices = this.m_simplifiedGraph.NumVertices;
    }

    private void ComputeSimplifiedParents(
      int label,
      out bool hadParentsBeforeSimplification,
      out bool firstParentStillInSet)
    {
      this.m_parents.Clear();
      firstParentStillInSet = true;
      hadParentsBeforeSimplification = false;
      bool flag = true;
      foreach (int label1 in this.m_graph.OutNeighborsOfLabel(label))
      {
        hadParentsBeforeSimplification = true;
        int simplification = this.GetSimplification(label1);
        if (flag)
          firstParentStillInSet = simplification != -1;
        flag = false;
        if (simplification != -1)
          this.m_parents.Add(simplification);
      }
    }

    private void TryCollapseMerge(int label, ref bool firstParentStillInSet)
    {
      if (((this.m_parents.Count <= 1 ? 0 : (this.m_firstParentPath.Contains(label) ? 1 : 0)) & (firstParentStillInSet ? 1 : 0)) != 0)
      {
        bool flag = false;
        for (int index = 1; index < this.m_parents.Count; ++index)
        {
          if (this.m_firstParentPath.Contains(this.m_parents[index]))
          {
            ++this.m_statistics.NumReachableByFirstParent;
            this.m_parents[index] = -2;
            flag = true;
          }
        }
        if (flag)
          this.SwapParentLists(ref firstParentStillInSet);
      }
      if (this.m_parents.Count > 1)
      {
        bool flag = false;
        for (int index1 = 1; index1 < this.m_parents.Count; ++index1)
        {
          for (int index2 = 0; this.m_parents[index1] != -2 && index2 < index1; ++index2)
          {
            if (this.m_parents[index1] == this.m_parents[index2])
            {
              ++this.m_statistics.NumReachableBySimplification;
              this.m_parents[index1] = -2;
              flag = true;
            }
            else if (this.m_parents[index2] != -2)
            {
              int ancestryDepth1 = this.m_graph.GetAncestryDepth(this.m_parents[index1]);
              int ancestryDepth2 = this.m_graph.GetAncestryDepth(this.m_parents[index2]);
              ++this.m_statistics.NumReachabilityQueries;
              if (ancestryDepth1 < ancestryDepth2 && this.m_algorithm.CanReachLabels((IDirectedGraph<int, Sha1Id>) this.m_graph, this.m_parents[index2], this.m_parents[index1]))
              {
                this.m_parents[index1] = -2;
                flag = true;
              }
              else if (ancestryDepth2 < ancestryDepth1 && this.m_algorithm.CanReachLabels((IDirectedGraph<int, Sha1Id>) this.m_graph, this.m_parents[index1], this.m_parents[index2]))
              {
                if (index2 == 0)
                  firstParentStillInSet = false;
                this.m_parents[index2] = -2;
                flag = true;
              }
            }
          }
        }
        if (flag)
          this.SwapParentLists(ref firstParentStillInSet);
      }
      if (this.m_parents.Count <= 1)
        return;
      this.m_simplification[label] = label;
      this.m_changeTypes[label] = new ChangeAndObjectType(TfsGitChangeType.Edit, this.m_pathOracle.GetObjectType(label));
    }

    private void SwapParentLists(ref bool firstParentStillInSet)
    {
      this.m_parentsSwap.Clear();
      for (int index = 0; index < this.m_parents.Count; ++index)
      {
        if (this.m_parents[index] != -2)
          this.m_parentsSwap.Add(this.m_parents[index]);
        else if (index == 0)
          firstParentStillInSet = false;
      }
      List<int> parents = this.m_parents;
      this.m_parents = this.m_parentsSwap;
      this.m_parentsSwap = parents;
    }

    private void SimplifyIfOneParent(int label, bool firstParentStillInSet)
    {
      if (this.m_parents.Count != 1)
        return;
      int parent = this.m_parents[0];
      if ((firstParentStillInSet ? (this.m_pathOracle.IsTreesameToFirstParent(label) ? 1 : 0) : (this.m_pathOracle.IsPairTreesame(label, parent) ? 1 : 0)) != 0)
      {
        this.m_simplification[label] = parent;
        this.m_pathOracle.ClearCachedTreesameInfo(label);
      }
      else
      {
        this.m_simplification[label] = label;
        TfsGitChangeType changeType = !this.m_pathOracle.HasPath(label) ? TfsGitChangeType.Delete : (!this.m_pathOracle.HasPath(parent) ? TfsGitChangeType.Add : TfsGitChangeType.Edit);
        this.m_changeTypes[label] = new ChangeAndObjectType(changeType, this.m_pathOracle.GetObjectType(label));
      }
    }

    private void SimplifyIfZeroParents(int label, bool hadParentsBeforeSimplification)
    {
      if (this.m_parents.Count != 0)
        return;
      if ((hadParentsBeforeSimplification ? (!this.m_pathOracle.IsTreesameToFirstParent(label) ? 1 : 0) : (this.m_pathOracle.HasPath(label) ? 1 : 0)) != 0)
      {
        this.m_simplification[label] = label;
        this.m_changeTypes[label] = new ChangeAndObjectType(TfsGitChangeType.Add, this.m_pathOracle.GetObjectType(label));
      }
      else
      {
        this.m_simplification[label] = -1;
        this.m_pathOracle.ClearCachedTreesameInfo(label);
      }
    }

    private int GetSimplification(int label)
    {
      int simplification;
      if (!this.m_simplification.TryGetValue(label, out simplification))
        simplification = label;
      return simplification;
    }
  }
}
